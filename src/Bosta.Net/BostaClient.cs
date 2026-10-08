using System;
using System.Net.Http;
using Bosta.Net.Clients;

namespace Bosta.Net
{
    /// <summary>
    /// The main entry point for the Bosta API.
    /// </summary>
    /// <example>
    /// <code>
    /// var client = new BostaClient("your-api-key");
    /// var delivery = await client.Deliveries.CreateAsync(new CreateDeliveryRequest
    /// {
    ///     Type = (int)DeliveryType.Send,
    ///     Cod = 250,
    ///     Receiver = new Receiver { FirstName = "Ahmed", Phone = "01001234567" },
    ///     DropOffAddress = new DeliveryAddress { City = "Cairo", FirstLine = "12 Tahrir St" }
    /// });
    /// </code>
    /// </example>
    public class BostaClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly bool _ownsHttpClient;
        private bool _disposed;

        /// <summary>
        /// Deliveries (shipments) API.
        /// </summary>
        public DeliveryClient Deliveries { get; }

        /// <summary>
        /// Pickup requests API.
        /// </summary>
        public PickupClient Pickups { get; }

        /// <summary>
        /// Cities, zones and districts API.
        /// </summary>
        public CityClient Cities { get; }

        /// <summary>
        /// Business pickup locations API.
        /// </summary>
        public PickupLocationClient PickupLocations { get; }

        /// <summary>
        /// Shipment pricing calculator API.
        /// </summary>
        public PricingClient Pricing { get; }

        /// <summary>
        /// Creates a client with the given API key.
        /// </summary>
        /// <param name="apiKey">Your Bosta API key.</param>
        /// <param name="baseUrl">Optional custom base URL (defaults to the production API).</param>
        public BostaClient(string apiKey, string baseUrl = null)
            : this(new BostaOptions { ApiKey = apiKey, BaseUrl = baseUrl ?? "https://app.bosta.co/api/v2/" }, null)
        {
        }

        /// <summary>
        /// Creates a client with the given options.
        /// </summary>
        public BostaClient(BostaOptions options)
            : this(options, null)
        {
        }

        /// <summary>
        /// Creates a client with the given options and a custom <see cref="HttpClient"/>
        /// (useful for testing or when integrating with <c>IHttpClientFactory</c>).
        /// </summary>
        public BostaClient(BostaOptions options, HttpClient httpClient)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.Validate();

            if (httpClient != null)
            {
                _http = httpClient;
                _ownsHttpClient = false;
            }
            else
            {
                _http = new HttpClient { BaseAddress = new Uri(EnsureTrailingSlash(options.BaseUrl)), Timeout = options.Timeout };
                _ownsHttpClient = true;
            }

            var inner = new BostaHttpClient(_http, options.ApiKey);
            Deliveries = new DeliveryClient(inner);
            Pickups = new PickupClient(inner);
            Cities = new CityClient(inner);
            PickupLocations = new PickupLocationClient(inner);
            Pricing = new PricingClient(inner);
        }

        private static string EnsureTrailingSlash(string url)
        {
            return url.EndsWith("/") ? url : url + "/";
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_ownsHttpClient) _http.Dispose();
        }
    }
}
