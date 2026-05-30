namespace NZWalks.API.Exceptions
{
    /// <summary>
    /// Custom exception class for application-specific exceptions.
    /// Used when business logic validation fails or expected errors occur.
    /// </summary>
    public class CustomException : Exception
    {
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }

        public CustomException(string message, int statusCode = 400, string errorCode = "INTERNAL_ERROR") 
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public CustomException(string message, Exception innerException, 
            int statusCode = 400, string errorCode = "INTERNAL_ERROR") 
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }

    /// <summary>
    /// Exception thrown when a requested resource is not found.
    /// </summary>
    public class ResourceNotFoundException : CustomException
    {
        public ResourceNotFoundException(string resourceName, string identifier) 
            : base($"{resourceName} with identifier '{identifier}' was not found.", 404, "NOT_FOUND")
        {
        }
    }

    /// <summary>
    /// Exception thrown when validation fails.
    /// </summary>
    public class ValidationException : CustomException
    {
        public ValidationException(string message) 
            : base(message, 400, "VALIDATION_ERROR")
        {
        }
    }

    /// <summary>
    /// Exception thrown when an operation is not authorized.
    /// </summary>
    public class UnauthorizedException : CustomException
    {
        public UnauthorizedException(string message = "Unauthorized access") 
            : base(message, 401, "UNAUTHORIZED")
        {
        }
    }

    /// <summary>
    /// Exception thrown when an operation is forbidden.
    /// </summary>
    public class ForbiddenException : CustomException
    {
        public ForbiddenException(string message = "Access forbidden") 
            : base(message, 403, "FORBIDDEN")
        {
        }
    }
}
