using NZWalks.API.Exceptions;
using NZWalks.API.Models.DTO;
using System.Net;
using System.Text.Json;

namespace NZWalks.API.Middlewares
{
    /// <summary>
    /// Global exception handling middleware that catches all unhandled exceptions
    /// in the request pipeline and returns a standardized error response.
    /// 
    /// This middleware is placed early in the pipeline to catch exceptions from
    /// all downstream components.
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, 
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An unhandled exception has occurred: {ExceptionMessage}", 
                    exception.Message);

                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponseDto
            {
                Path = context.Request.Path,
                Timestamp = DateTime.UtcNow
            };

            // Handle different exception types
            if (exception is CustomException customException)
            {
                context.Response.StatusCode = customException.StatusCode;
                response.StatusCode = customException.StatusCode;
                response.ErrorCode = customException.ErrorCode;
                response.Message = customException.Message;

                _logger.LogWarning("Handled custom exception: {ErrorCode} - {Message}", 
                    customException.ErrorCode, customException.Message);
            }
            else if (exception is ArgumentNullException argNullEx)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ErrorCode = "ARGUMENT_NULL";
                response.Message = $"Required argument is null: {argNullEx.ParamName}";
            }
            else if (exception is ArgumentException argEx)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ErrorCode = "INVALID_ARGUMENT";
                response.Message = argEx.Message;
            }
            else if (exception is UnauthorizedAccessException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.ErrorCode = "UNAUTHORIZED";
                response.Message = "Unauthorized access.";
            }
            else if (exception is NotImplementedException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                response.StatusCode = (int)HttpStatusCode.NotImplemented;
                response.ErrorCode = "NOT_IMPLEMENTED";
                response.Message = "The requested operation is not implemented.";
            }
            else if (exception is TimeoutException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                response.ErrorCode = "TIMEOUT";
                response.Message = "The request timed out. Please try again.";
            }
            else
            {
                // Generic/unhandled exception
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorCode = "INTERNAL_SERVER_ERROR";

                // In development, include detailed error information
                if (_environment.IsDevelopment())
                {
                    response.Message = exception.Message;
                    response.StackTrace = exception.StackTrace;
                    response.Details["InnerException"] = exception.InnerException?.Message;
                }
                else
                {
                    // In production, return generic message
                    response.Message = "An internal server error occurred. Please contact support.";
                }

                _logger.LogError(exception, "Unhandled exception occurred");
            }

            // Include stack trace in development environment for debugging
            if (_environment.IsDevelopment() && string.IsNullOrEmpty(response.StackTrace))
            {
                response.StackTrace = exception.StackTrace;
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, jsonOptions);

            return context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// Extension method to add global exception handling middleware to the pipeline.
    /// </summary>
    public static class GlobalExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}
