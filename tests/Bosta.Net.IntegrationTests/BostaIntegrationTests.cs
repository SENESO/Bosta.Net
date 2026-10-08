using System;
using System.Threading.Tasks;
using Bosta.Net;
using Bosta.Net.Models;
using Xunit;
using Xunit.Abstractions;

namespace Bosta.Net.IntegrationTests
{
    /// <summary>
    /// Live integration tests against the real Bosta API.
    /// <para>
    /// These tests require a <c>BOSTA_API_KEY</c> environment variable.
    /// They return early (pass) when the key is missing, and they only
    /// perform read-only calls — no deliveries are created.
    /// </para>
    /// </summary>
    public class BostaIntegrationTests
    {
        private readonly ITestOutputHelper _output;

        public BostaIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private BostaClient TryCreateClient()
        {
            var apiKey = Environment.GetEnvironmentVariable("BOSTA_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _output.WriteLine("BOSTA_API_KEY is not set — skipping live test.");
                return null;
            }
            return new BostaClient(apiKey);
        }

        [Fact]
        public async Task ListCities_ReturnsCities()
        {
            var client = TryCreateClient();
            if (client == null) return;

            var cities = await client.Cities.ListAsync();
            Assert.NotNull(cities);
            Assert.NotEmpty(cities);
        }

        [Fact]
        public async Task GetZones_ForFirstCity_ReturnsZones()
        {
            var client = TryCreateClient();
            if (client == null) return;

            var cities = await client.Cities.ListAsync();
            var zones = await client.Cities.GetZonesAsync(cities[0].Id);
            Assert.NotNull(zones);
        }

        [Fact]
        public async Task ListPickupLocations_DoesNotThrow()
        {
            var client = TryCreateClient();
            if (client == null) return;

            var locations = await client.PickupLocations.ListAsync();
            Assert.NotNull(locations);
        }

        [Fact]
        public async Task InvalidApiKey_ThrowsBostaApiException()
        {
            var client = TryCreateClient();
            if (client == null) return;

            var badClient = new BostaClient("invalid-key-for-test");
            var ex = await Assert.ThrowsAsync<BostaApiException>(() => badClient.Cities.ListAsync());
            Assert.False(string.IsNullOrWhiteSpace(ex.Message));
        }
    }
}
