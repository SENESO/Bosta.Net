using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bosta.Net.Models;

namespace Bosta.Net.Clients
{
    /// <summary>
    /// Deliveries (shipments) API.
    /// </summary>
    public class DeliveryClient
    {
        private readonly BostaHttpClient _http;

        internal DeliveryClient(BostaHttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Creates a new delivery.
        /// </summary>
        /// <param name="request">The delivery details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created delivery's ID and tracking number.</returns>
        public Task<CreateDeliveryResponse> CreateAsync(CreateDeliveryRequest request, CancellationToken cancellationToken = default)
        {
            return _http.PostAsync<CreateDeliveryResponse>("deliveries?apiVersion=1", request, cancellationToken);
        }

        /// <summary>
        /// Gets a delivery by tracking number (includes current state and history).
        /// </summary>
        public Task<Delivery> GetAsync(string trackingNumber, CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<Delivery>("deliveries/business/" + trackingNumber, cancellationToken);
        }

        /// <summary>
        /// Tracks a delivery — returns its current state plus the full tracking history.
        /// </summary>
        public Task<Delivery> TrackAsync(string trackingNumber, CancellationToken cancellationToken = default)
        {
            return GetAsync(trackingNumber, cancellationToken);
        }

        /// <summary>
        /// Terminates (cancels) a delivery by tracking number.
        /// Note: this is a hard terminate — the delivery cannot be resumed.
        /// </summary>
        public Task<string> TerminateAsync(string trackingNumber, CancellationToken cancellationToken = default)
        {
            return _http.DeleteAsync<string>("deliveries/business/" + trackingNumber + "/terminate", cancellationToken);
        }

        /// <summary>
        /// Searches deliveries by tracking numbers and/or business reference.
        /// </summary>
        public Task<List<Delivery>> SearchAsync(SearchDeliveriesRequest request, CancellationToken cancellationToken = default)
        {
            return _http.PostAsync<List<Delivery>>("deliveries/search", request, cancellationToken);
        }

        /// <summary>
        /// Creates multiple deliveries in one call.
        /// </summary>
        public Task<List<CreateDeliveryResponse>> CreateBulkAsync(List<CreateDeliveryRequest> requests, CancellationToken cancellationToken = default)
        {
            return _http.PostAsync<List<CreateDeliveryResponse>>("deliveries/bulk?apiVersion=1", requests, cancellationToken);
        }

        /// <summary>
        /// Prints airway bills for the given tracking numbers (returns a base64-encoded PDF, or a URL).
        /// </summary>
        public Task<PrintAwbResponse> PrintAwbAsync(PrintAwbRequest request, CancellationToken cancellationToken = default)
        {
            return _http.PostAsync<PrintAwbResponse>("deliveries/mass-awb", request, cancellationToken);
        }
    }
}
