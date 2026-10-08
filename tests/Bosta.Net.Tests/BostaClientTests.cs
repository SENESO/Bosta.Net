using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bosta.Net;
using Bosta.Net.Models;
using Bosta.Net.Webhooks;
using Xunit;

namespace Bosta.Net.Tests
{
    /// <summary>
    /// A mock HTTP handler that returns canned responses and records requests.
    /// </summary>
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public HttpRequestMessage LastRequest { get; private set; }
        public string LastRequestBody { get; private set; }

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (request.Content != null)
                LastRequestBody = await request.Content.ReadAsStringAsync();
            return _responder(request);
        }

        public static HttpResponseMessage Json(string json, HttpStatusCode status = HttpStatusCode.OK)
        {
            return new HttpResponseMessage(status)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }

    public class BostaClientTests
    {
        private static BostaClient CreateClient(MockHttpMessageHandler handler)
        {
            var http = new HttpClient(handler) { BaseAddress = new Uri("https://app.bosta.co/api/v2/") };
            return new BostaClient(new BostaOptions { ApiKey = "test-key" }, http);
        }

        [Fact]
        public async Task CreateDelivery_SendsCorrectRequest_AndParsesResponse()
        {
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":true,\"message\":\"Delivery created\",\"data\":{\"_id\":\"abc123\",\"trackingNumber\":\"12345678\",\"businessReference\":\"ORD-1\"}}"));
            var client = CreateClient(handler);

            var result = await client.Deliveries.CreateAsync(new CreateDeliveryRequest
            {
                Type = (int)DeliveryType.Send,
                Cod = 250,
                Receiver = new Receiver { FirstName = "Ahmed", Phone = "01001234567" },
                DropOffAddress = new DeliveryAddress { City = "Cairo", FirstLine = "12 Tahrir St" },
                BusinessReference = "ORD-1"
            });

            Assert.Equal("abc123", result.Id);
            Assert.Equal("12345678", result.TrackingNumber);
            Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
            Assert.EndsWith("deliveries?apiVersion=1", handler.LastRequest.RequestUri.ToString());
            // Raw API key — no Bearer prefix
            Assert.Equal("test-key", string.Join(",", handler.LastRequest.Headers.GetValues("Authorization")));
            Assert.Contains("\"businessReference\":\"ORD-1\"", handler.LastRequestBody);
        }

        [Fact]
        public async Task CreateDelivery_OmitsNullFields_FromJson()
        {
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":true,\"message\":\"ok\",\"data\":{\"_id\":\"x\",\"trackingNumber\":\"1\"}}"));
            var client = CreateClient(handler);

            await client.Deliveries.CreateAsync(new CreateDeliveryRequest
            {
                Type = 10,
                Cod = 0,
                Receiver = new Receiver { FirstName = "A", Phone = "01" },
                DropOffAddress = new DeliveryAddress { City = "Cairo", FirstLine = "x" }
            });

            Assert.DoesNotContain("notes", handler.LastRequestBody);
            Assert.DoesNotContain("pickupAddress", handler.LastRequestBody);
            Assert.DoesNotContain("returnAddress", handler.LastRequestBody);
        }

        [Fact]
        public async Task GetDelivery_ParsesStateAndHistory()
        {
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":true,\"message\":\"ok\",\"data\":{" +
                "\"_id\":\"abc123\",\"trackingNumber\":\"12345678\",\"type\":10,\"cod\":250," +
                "\"state\":{\"code\":45,\"value\":\"Delivered\"}," +
                "\"history\":[{\"state\":{\"code\":22,\"value\":\"Picked up\"},\"timestamp\":\"2026-10-01T10:00:00Z\"}," +
                "{\"state\":{\"code\":45,\"value\":\"Delivered\"},\"timestamp\":\"2026-10-02T14:00:00Z\"}]}}"));
            var client = CreateClient(handler);

            var delivery = await client.Deliveries.TrackAsync("12345678");

            Assert.Equal("12345678", delivery.TrackingNumber);
            Assert.Equal(45, delivery.State.Code);
            Assert.Equal("Delivered", delivery.State.Value);
            Assert.Equal(2, delivery.History.Count);
            Assert.True(delivery.IsTerminal);
        }

        [Fact]
        public async Task TerminateDelivery_UsesDeleteEndpoint()
        {
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":true,\"message\":\"Delivery terminated\",\"data\":\"Delivery terminated\"}"));
            var client = CreateClient(handler);

            await client.Deliveries.TerminateAsync("12345678");

            Assert.Equal(HttpMethod.Delete, handler.LastRequest.Method);
            Assert.EndsWith("deliveries/business/12345678/terminate", handler.LastRequest.RequestUri.ToString());
        }

        [Fact]
        public async Task ApiError_ThrowsBostaApiException_WithMessage()
        {
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":false,\"message\":\"Invalid API key\",\"code\":401}",
                HttpStatusCode.Unauthorized));
            var client = CreateClient(handler);

            var ex = await Assert.ThrowsAsync<BostaApiException>(() =>
                client.Deliveries.GetAsync("12345678"));

            Assert.Equal("Invalid API key", ex.Message);
            Assert.Equal(401, ex.StatusCode);
        }

        [Fact]
        public async Task SuccessFalse_OnHttp200_ThrowsBostaApiException()
        {
            // Bosta returns HTTP 200 with success=false on business errors.
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":false,\"message\":\"City not found\"}"));
            var client = CreateClient(handler);

            var ex = await Assert.ThrowsAsync<BostaApiException>(() =>
                client.Cities.GetAsync("bad-id"));

            Assert.Equal("City not found", ex.Message);
        }

        [Fact]
        public async Task ListCities_ParsesArray()
        {
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":true,\"message\":\"ok\",\"data\":[" +
                "{\"_id\":\"city1\",\"name\":\"Cairo\",\"countryId\":\"60e4482c7cb7d4bc4849c4d5\"}," +
                "{\"_id\":\"city2\",\"name\":\"Alexandria\",\"countryId\":\"60e4482c7cb7d4bc4849c4d5\"}]}"));
            var client = CreateClient(handler);

            var cities = await client.Cities.ListAsync();

            Assert.Equal(2, cities.Count);
            Assert.Equal("Cairo", cities[0].Name);
            Assert.EndsWith("cities", handler.LastRequest.RequestUri.ToString());
        }

        [Fact]
        public async Task GetZones_UsesCityIdInPath()
        {
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":true,\"message\":\"ok\",\"data\":[{\"_id\":\"z1\",\"name\":\"Nasr City\"}]}"));
            var client = CreateClient(handler);

            var zones = await client.Cities.GetZonesAsync("city1");

            Assert.Single(zones);
            Assert.EndsWith("cities/city1/zones", handler.LastRequest.RequestUri.ToString());
        }

        [Fact]
        public async Task CreatePickup_SendsScheduledFields()
        {
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":true,\"message\":\"ok\",\"data\":{\"_id\":\"p1\",\"scheduledDate\":\"2026-10-09\",\"state\":\"Scheduled\"}}"));
            var client = CreateClient(handler);

            var pickup = await client.Pickups.CreateAsync(new CreatePickupRequest
            {
                ScheduledDate = "2026-10-09",
                ScheduledTimeSlot = "10:00-13:00",
                NumberOfPackages = 3,
                Notes = "Call on arrival"
            });

            Assert.Equal("p1", pickup.Id);
            Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
            Assert.Contains("\"scheduledDate\":\"2026-10-09\"", handler.LastRequestBody);
            Assert.Contains("\"noOfPackages\":3", handler.LastRequestBody);
        }

        [Fact]
        public async Task ListPickupLocations_CallsCorrectEndpoint()
        {
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":true,\"message\":\"ok\",\"data\":[{\"_id\":\"l1\",\"locationName\":\"Warehouse\",\"isDefault\":true}]}"));
            var client = CreateClient(handler);

            var locations = await client.PickupLocations.ListAsync();

            Assert.Single(locations);
            Assert.True(locations[0].IsDefault);
            Assert.EndsWith("pickup-locations", handler.LastRequest.RequestUri.ToString());
        }

        [Fact]
        public async Task PrintAwb_SendsTrackingNumbers()
        {
            var handler = new MockHttpMessageHandler(_ => MockHttpMessageHandler.Json(
                "{\"success\":true,\"message\":\"ok\",\"data\":{\"awb\":\"base64pdf...\",\"url\":\"https://x/awb.pdf\"}}"));
            var client = CreateClient(handler);

            var awb = await client.Deliveries.PrintAwbAsync(new PrintAwbRequest
            {
                TrackingNumbers = "111,222",
                Size = AwbSizes.A4,
                Language = "ar"
            });

            Assert.Equal("base64pdf...", awb.AwbBase64);
            Assert.EndsWith("deliveries/mass-awb", handler.LastRequest.RequestUri.ToString());
            Assert.Contains("\"trackingNumbers\":\"111,222\"", handler.LastRequestBody);
        }

        [Fact]
        public void MissingApiKey_ThrowsArgumentException()
        {
            Assert.Throws<System.ArgumentException>(() => new BostaClient(""));
            Assert.Throws<System.ArgumentException>(() => new BostaClient(new BostaOptions()));
        }

        [Fact]
        public void WebhookValidator_AcceptsMatchingSecrets()
        {
            Assert.True(BostaWebhookValidator.IsValid("s3cr3t", "s3cr3t"));
        }

        [Fact]
        public void WebhookValidator_RejectsMismatch_AndFailsClosed()
        {
            Assert.False(BostaWebhookValidator.IsValid("wrong", "s3cr3t"));
            Assert.False(BostaWebhookValidator.IsValid(null, "s3cr3t"));
            Assert.False(BostaWebhookValidator.IsValid("s3cr3t", null));
            Assert.False(BostaWebhookValidator.IsValid("", ""));
        }

        [Fact]
        public void DeliveryType_HasCorrectCodes()
        {
            Assert.Equal(10, (int)DeliveryType.Send);
            Assert.Equal(15, (int)DeliveryType.CashCollection);
            Assert.Equal(25, (int)DeliveryType.CustomerReturnPickup);
            Assert.Equal(30, (int)DeliveryType.Exchange);
        }
    }
}
