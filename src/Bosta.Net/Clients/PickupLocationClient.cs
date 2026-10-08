using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bosta.Net.Models;

namespace Bosta.Net.Clients
{
    /// <summary>
    /// Business pickup locations API.
    /// </summary>
    public class PickupLocationClient
    {
        private readonly BostaHttpClient _http;

        internal PickupLocationClient(BostaHttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Lists the business's pickup locations.
        /// </summary>
        public Task<List<PickupLocation>> ListAsync(CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<List<PickupLocation>>("pickup-locations", cancellationToken);
        }

        /// <summary>
        /// Gets a pickup location by ID.
        /// </summary>
        public Task<PickupLocation> GetAsync(string locationId, CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<PickupLocation>("pickup-locations/" + locationId, cancellationToken);
        }
    }
}
