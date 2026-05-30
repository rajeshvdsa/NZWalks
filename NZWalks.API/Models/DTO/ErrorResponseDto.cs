namespace NZWalks.API.Models.DTO
{
    /// <summary>
    /// Standard error response DTO used for all error responses in the API.
    /// Provides consistent error information to clients.
    /// </summary>
    public class ErrorResponseDto
    {
        /// <summary>
        /// Unique error code for programmatic error handling
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Human-readable error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// HTTP status code of the error
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Timestamp when the error occurred
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Request path that caused the error (useful for debugging)
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Stack trace (only included in development environment)
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Additional error details (can include validation errors, etc.)
        /// </summary>
        public Dictionary<string, object> Details { get; set; }

        public ErrorResponseDto()
        {
            Timestamp = DateTime.UtcNow;
            Details = new Dictionary<string, object>();
        }

        public ErrorResponseDto(string errorCode, string message, int statusCode, string path = null) 
            : this()
        {
            ErrorCode = errorCode;
            Message = message;
            StatusCode = statusCode;
            Path = path;
        }
    }
}
