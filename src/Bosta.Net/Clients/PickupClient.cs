using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bosta.Net.Models;

namespace Bosta.Net.Clients
{
    /// <summary>
    /// Pickup requests API — schedule couriers to collect shipments from your location.
    /// </summary>
    public class PickupClient
    {
        private readonly BostaHttpClient _http;

        internal PickupClient(BostaHttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Lists pickup requests.
        /// </summary>
        public Task<List<PickupRequest>> ListAsync(int pageId = 0, CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<List<PickupRequest>>("pickups?pageId=" + pageId, cancellationToken);
        }

        /// <summary>
        /// Gets a pickup request by ID.
        /// </summary>
        public Task<PickupRequest> GetAsync(string pickupRequestId, CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<PickupRequest>("pickups/" + pickupRequestId, cancellationToken);
        }

        /// <summary>
        /// Creates a pickup request.
        /// </summary>
        public Task<PickupRequest> CreateAsync(CreatePickupRequest request, CancellationToken cancellationToken = default)
        {
            return _http.PostAsync<PickupRequest>("pickups", request, cancellationToken);
        }

        /// <summary>
        /// Updates a pickup request.
        /// </summary>
        public Task<PickupRequest> UpdateAsync(string pickupRequestId, CreatePickupRequest request, CancellationToken cancellationToken = default)
        {
            return _http.PutAsync<PickupRequest>("pickups/" + pickupRequestId, request, cancellationToken);
        }

        /// <summary>
        /// Deletes a pickup request.
        /// </summary>
        public Task<string> DeleteAsync(string pickupRequestId, CancellationToken cancellationToken = default)
        {
            return _http.DeleteAsync<string>("pickups/" + pickupRequestId, cancellationToken);
        }

        /// <summary>
        /// Returns the dates available for scheduling a pickup.
        /// </summary>
        public Task<List<string>> GetAvailableDatesAsync(CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<List<string>>("pickups/available-dates", cancellationToken);
        }
    }
}
