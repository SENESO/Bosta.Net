using System;

namespace Bosta.Net.Webhooks
{
    /// <summary>
    /// Validates incoming Bosta webhooks.
    /// <para>
    /// Bosta does not sign webhook payloads with HMAC. Instead, you register a
    /// secret in the Bosta dashboard and Bosta includes it with every webhook call
    /// (as a query-string parameter or header). Validate it with a constant-time
    /// comparison and fail closed when no secret is configured.
    /// </para>
    /// </summary>
    public static class BostaWebhookValidator
    {
        /// <summary>
        /// Validates the secret Bosta sent against the secret you configured.
        /// Uses a constant-time comparison to avoid timing attacks.
        /// Returns false (fail closed) when either value is missing.
        /// </summary>
        /// <param name="receivedSecret">The secret from the incoming webhook request.</param>
        /// <param name="configuredSecret">The secret you registered in the Bosta dashboard.</param>
        public static bool IsValid(string receivedSecret, string configuredSecret)
        {
            if (string.IsNullOrEmpty(receivedSecret) || string.IsNullOrEmpty(configuredSecret))
                return false;

            var a = System.Text.Encoding.UTF8.GetBytes(receivedSecret);
            var b = System.Text.Encoding.UTF8.GetBytes(configuredSecret);
            if (a.Length != b.Length) return false;

            var diff = 0;
            for (var i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
