using System.Text.Json.Serialization;

namespace Bosta.Net.Webhooks
{
    /// <summary>
    /// A webhook event sent by Bosta when a delivery changes state.
    /// Register the webhook URL in the Bosta dashboard; Bosta POSTs this payload
    /// with your dashboard-registered secret (query string or header).
    /// </summary>
    public class BostaWebhookEvent
    {
        [JsonPropertyName("_id")]
        public string DeliveryId { get; set; }

        [JsonPropertyName("trackingNumber")]
        public string TrackingNumber { get; set; }

        /// <summary>Numeric delivery state code (e.g. 45 = delivered).</summary>
        [JsonPropertyName("state")]
        public int State { get; set; }

        /// <summary>Delivery type, e.g. "SEND".</summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>Unix timestamp of the event.</summary>
        [JsonPropertyName("timeStamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("cod")]
        public decimal? Cod { get; set; }

        [JsonPropertyName("businessReference")]
        public string BusinessReference { get; set; }
    }
}
