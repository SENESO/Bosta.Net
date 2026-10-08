using System;

namespace Bosta.Net
{
    /// <summary>
    /// Thrown when the Bosta API returns an error response.
    /// </summary>
    public class BostaApiException : Exception
    {
        /// <summary>
        /// The error code returned by the API, if any.
        /// </summary>
        public int? ErrorCode { get; }

        /// <summary>
        /// The HTTP status code of the failed request, if available.
        /// </summary>
        public int? StatusCode { get; }

        public BostaApiException(string message, int? errorCode = null, int? statusCode = null)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        public BostaApiException(string message, Exception innerException, int? errorCode = null, int? statusCode = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}
