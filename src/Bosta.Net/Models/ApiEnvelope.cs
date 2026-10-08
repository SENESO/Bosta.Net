using System.Text.Json.Serialization;

namespace Bosta.Net.Models
{
    /// <summary>
    /// The envelope every Bosta API response is wrapped in.
    /// </summary>
    /// <typeparam name="T">The payload type.</typeparam>
    public class ApiEnvelope<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }
}
