using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bosta.Net.Models;

namespace Bosta.Net.Clients
{
    /// <summary>
    /// Internal HTTP plumbing shared by all API clients.
    /// </summary>
    internal class BostaHttpClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private readonly HttpClient _http;
        private readonly string _apiKey;

        public BostaHttpClient(HttpClient http, string apiKey)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<T> GetAsync<T>(string path, CancellationToken ct)
        {
            using (var request = CreateRequest(HttpMethod.Get, path))
            using (var response = await _http.SendAsync(request, ct).ConfigureAwait(false))
            {
                return await ReadEnvelopeAsync<T>(response).ConfigureAwait(false);
            }
        }

        public async Task<T> PostAsync<T>(string path, object body, CancellationToken ct)
        {
            using (var request = CreateRequest(HttpMethod.Post, path, body))
            using (var response = await _http.SendAsync(request, ct).ConfigureAwait(false))
            {
                return await ReadEnvelopeAsync<T>(response).ConfigureAwait(false);
            }
        }

        public async Task<T> PutAsync<T>(string path, object body, CancellationToken ct)
        {
            using (var request = CreateRequest(HttpMethod.Put, path, body))
            using (var response = await _http.SendAsync(request, ct).ConfigureAwait(false))
            {
                return await ReadEnvelopeAsync<T>(response).ConfigureAwait(false);
            }
        }

        public async Task<T> DeleteAsync<T>(string path, CancellationToken ct)
        {
            using (var request = CreateRequest(HttpMethod.Delete, path))
            using (var response = await _http.SendAsync(request, ct).ConfigureAwait(false))
            {
                return await ReadEnvelopeAsync<T>(response).ConfigureAwait(false);
            }
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string path, object body = null)
        {
            var request = new HttpRequestMessage(method, path);
            // Bosta expects the raw API key — no "Bearer" prefix.
            request.Headers.TryAddWithoutValidation("Authorization", _apiKey);
            request.Headers.TryAddWithoutValidation("X-Requested-By", "bosta-net-sdk");
            if (body != null)
            {
                var json = JsonSerializer.Serialize(body, JsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            return request;
        }

        private static async Task<T> ReadEnvelopeAsync<T>(HttpResponseMessage response)
        {
            var raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            ApiEnvelope<T> envelope = null;
            try
            {
                envelope = JsonSerializer.Deserialize<ApiEnvelope<T>>(raw, JsonOptions);
            }
            catch (JsonException ex)
            {
                throw new BostaApiException(
                    $"Bosta API returned an unreadable response (HTTP {(int)response.StatusCode}).",
                    ex,
                    statusCode: (int)response.StatusCode);
            }

            if (envelope == null)
            {
                throw new BostaApiException(
                    $"Bosta API returned an empty response (HTTP {(int)response.StatusCode}).",
                    statusCode: (int)response.StatusCode);
            }

            // Bosta returns HTTP 200 with success=false on business errors.
            if (!envelope.Success)
            {
                throw new BostaApiException(
                    string.IsNullOrWhiteSpace(envelope.Message)
                        ? $"Bosta API request failed (HTTP {(int)response.StatusCode})."
                        : envelope.Message,
                    errorCode: envelope.Code,
                    statusCode: (int)response.StatusCode);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new BostaApiException(
                    string.IsNullOrWhiteSpace(envelope.Message)
                        ? $"Bosta API request failed (HTTP {(int)response.StatusCode})."
                        : envelope.Message,
                    errorCode: envelope.Code,
                    statusCode: (int)response.StatusCode);
            }

            return envelope.Data;
        }
    }
}
