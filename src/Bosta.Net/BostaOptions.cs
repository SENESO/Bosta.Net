using System;

namespace Bosta.Net
{
    /// <summary>
    /// Configuration options for <see cref="BostaClient"/>.
    /// </summary>
    public class BostaOptions
    {
        /// <summary>
        /// Your Bosta API key (from the Bosta dashboard). Sent as the raw
        /// <c>Authorization</c> header — no <c>Bearer</c> prefix.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Base URL of the Bosta API. Defaults to <c>https://app.bosta.co/api/v2/</c>.
        /// </summary>
        public string BaseUrl { get; set; } = "https://app.bosta.co/api/v2/";

        /// <summary>
        /// HTTP timeout for API calls. Defaults to 30 seconds.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new ArgumentException("Bosta API key is required.", nameof(ApiKey));
            if (string.IsNullOrWhiteSpace(BaseUrl))
                throw new ArgumentException("Bosta base URL is required.", nameof(BaseUrl));
        }
    }
}
