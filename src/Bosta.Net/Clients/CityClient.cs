using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bosta.Net.Models;

namespace Bosta.Net.Clients
{
    /// <summary>
    /// Cities, zones and districts API.
    /// </summary>
    public class CityClient
    {
        private readonly BostaHttpClient _http;

        internal CityClient(BostaHttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Lists all cities served by Bosta.
        /// </summary>
        public Task<List<City>> ListAsync(CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<List<City>>("cities", cancellationToken);
        }

        /// <summary>
        /// Gets a city by ID.
        /// </summary>
        public Task<City> GetAsync(string cityId, CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<City>("cities/" + cityId, cancellationToken);
        }

        /// <summary>
        /// Lists the zones of a city.
        /// </summary>
        public Task<List<Zone>> GetZonesAsync(string cityId, CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<List<Zone>>("cities/" + cityId + "/zones", cancellationToken);
        }

        /// <summary>
        /// Lists the districts of a city.
        /// </summary>
        public Task<List<District>> GetDistrictsAsync(string cityId, CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<List<District>>("cities/" + cityId + "/districts", cancellationToken);
        }

        /// <summary>
        /// Lists all districts across all cities.
        /// </summary>
        public Task<List<District>> GetAllDistrictsAsync(CancellationToken cancellationToken = default)
        {
            return _http.GetAsync<List<District>>("cities/getAllDistricts", cancellationToken);
        }
    }
}
