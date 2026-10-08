using System.Threading;
using System.Threading.Tasks;
using Bosta.Net.Models;

namespace Bosta.Net.Clients
{
    /// <summary>
    /// Shipment pricing calculator API.
    /// </summary>
    public class PricingClient
    {
        private readonly BostaHttpClient _http;

        internal PricingClient(BostaHttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Estimates the shipping price for a shipment.
        /// </summary>
        /// <param name="cityId">Destination city ID (from <see cref="CityClient"/>).</param>
        /// <param name="zoneId">Destination zone ID (optional).</param>
        /// <param name="size">Package size (see <see cref="PackageSizes"/>).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task<PriceEstimate> CalculateShipmentPriceAsync(
            string cityId,
            string zoneId = null,
            string size = PackageSizes.Medium,
            CancellationToken cancellationToken = default)
        {
            var path = "pricing/shipment/calculator?cityId=" + cityId + "&size=" + size;
            if (!string.IsNullOrWhiteSpace(zoneId))
                path += "&zoneId=" + zoneId;
            return _http.GetAsync<PriceEstimate>(path, cancellationToken);
        }
    }
}
