namespace NZWalks.API.Examples
{
    /*
    ====================================================================
    GLOBAL EXCEPTION HANDLING - USAGE EXAMPLES
    ====================================================================

    This file demonstrates how to use the global exception handling system
    implemented in your NZWalks API. The middleware will catch ALL exceptions
    and return a standardized error response.

    ====================================================================

    EXAMPLE 1: Using ResourceNotFoundException in a Controller
    ====================================================================

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var region = await regionRepository.GetByIdAsync(id);

        // Throw custom exception instead of returning NotFound()
        if (region == null)
        {
            throw new ResourceNotFoundException("Region", id.ToString());
            // This will automatically return:
            // {
            //   "errorCode": "NOT_FOUND",
            //   "message": "Region with identifier 'guid-value' was not found.",
            //   "statusCode": 404,
            //   "timestamp": "2024-01-15T10:30:00Z",
            //   "path": "/api/regions/guid-value"
            // }
        }

        var regionDto = mapper.Map<RegionDto>(region);
        return Ok(regionDto);
    }


    ====================================================================

    EXAMPLE 2: Using ValidationException for Business Logic Validation
    ====================================================================

    [HttpPost]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto request)
    {
        // Validate business logic
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Region name cannot be empty");
            // Returns:
            // {
            //   "errorCode": "VALIDATION_ERROR",
            //   "message": "Region name cannot be empty",
            //   "statusCode": 400,
            //   "timestamp": "2024-01-15T10:30:00Z",
            //   "path": "/api/regions"
            // }
        }

        if (request.Name.Length > 100)
        {
            throw new ValidationException("Region name cannot exceed 100 characters");
        }

        // Continue with creation...
        var region = mapper.Map<Region>(request);
        await regionRepository.AddAsync(region);

        return CreatedAtAction(nameof(GetById), new { id = region.Id }, 
            mapper.Map<RegionDto>(region));
    }


    ====================================================================

    EXAMPLE 3: Using UnauthorizedException for Permission Issues
    ====================================================================

    [HttpDelete("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var region = await regionRepository.GetByIdAsync(id);

        if (region == null)
        {
            throw new ResourceNotFoundException("Region", id.ToString());
        }

        // Check if user has permission
        var userId = User.FindFirst("sub")?.Value;
        if (region.CreatedBy != userId)
        {
            throw new UnauthorizedException("You do not have permission to delete this region");
            // Returns:
            // {
            //   "errorCode": "UNAUTHORIZED",
            //   "message": "You do not have permission to delete this region",
            //   "statusCode": 401,
            //   ...
            // }
        }

        await regionRepository.DeleteAsync(id);
        return Ok();
    }


    ====================================================================

    EXAMPLE 4: Unhandled Exception (Caught Automatically)
    ====================================================================

    public async Task<IActionResult> ComplexOperation()
    {
        // Imagine this throws an unexpected exception
        var result = await someService.PerformComplexCalculation();

        // The middleware will catch it and return:
        // DEVELOPMENT:
        // {
        //   "errorCode": "INTERNAL_SERVER_ERROR",
        //   "message": "Attempted to divide by zero",
        //   "statusCode": 500,
        //   "timestamp": "2024-01-15T10:30:00Z",
        //   "path": "/api/operations/complex",
        //   "stackTrace": "at SomeClass.PerformCalculation() in SomeFile.cs:line 42..."
        // }
        //
        // PRODUCTION:
        // {
        //   "errorCode": "INTERNAL_SERVER_ERROR",
        //   "message": "An internal server error occurred. Please contact support.",
        //   "statusCode": 500,
        //   "timestamp": "2024-01-15T10:30:00Z",
        //   "path": "/api/operations/complex"
        // }
    }


    ====================================================================

    KEY BENEFITS OF GLOBAL EXCEPTION HANDLING
    ====================================================================

    1. CONSISTENCY: All errors return the same format
       - Clients know exactly what to expect
       - Easy to parse and handle on frontend

    2. SECURITY: Never expose sensitive details in production
       - Stack traces hidden from users
       - Only generic error messages shown
       - Full details available in logs for debugging

    3. LOGGING: All exceptions are logged automatically
       - Easy to monitor and debug production issues
       - Error tracking and analytics

    4. DEVELOPER EXPERIENCE: Simple and intuitive
       - Just throw an exception, middleware handles the rest
       - No need for try-catch in every controller action
       - No need to manually serialize error responses

    5. MAINTAINABILITY: Centralized error handling logic
       - Changes to error response format only need to happen once
       - New exception types can be added easily
       - Business logic stays clean and focused


    ====================================================================

    HOW IT WORKS
    ====================================================================

    1. Exception is thrown anywhere in the request pipeline
    2. GlobalExceptionHandlingMiddleware catches it
    3. Exception type is determined
    4. Appropriate error response is generated:
       - CustomException types: Use their own status codes and messages
       - Known .NET exceptions: Map to appropriate HTTP status codes
       - Unknown exceptions: Return 500 with safe generic message
    5. Exception is logged
    6. Error response is returned to client as JSON


    ====================================================================

    CREATING CUSTOM EXCEPTION TYPES
    ====================================================================

    You can create domain-specific exceptions easily:

    public class InsufficientCreditsException : CustomException
    {
        public InsufficientCreditsException(decimal required, decimal available)
            : base(
                $"Insufficient credits. Required: {required}, Available: {available}",
                statusCode: 402, // Payment Required
                errorCode: "INSUFFICIENT_CREDITS"
            )
        {
        }
    }

    Then in your middleware, add handling:

    else if (exception is InsufficientCreditsException insufficientEx)
    {
        context.Response.StatusCode = insufficientEx.StatusCode;
        response.StatusCode = insufficientEx.StatusCode;
        response.ErrorCode = insufficientEx.ErrorCode;
        response.Message = insufficientEx.Message;
    }


    ====================================================================

    LOGGING
    ====================================================================

    The middleware logs all exceptions automatically:

    - CustomException (business logic errors): Logged as WARNING
    - Known .NET exceptions: Logged based on severity
    - Unhandled exceptions: Logged as ERROR

    You can inject ILogger in your services to log domain-specific events:

    public class RegionService
    {
        private readonly ILogger<RegionService> _logger;

        public RegionService(ILogger<RegionService> logger)
        {
            _logger = logger;
        }

        public async Task<Region> GetRegionAsync(Guid id)
        {
            _logger.LogInformation("Fetching region with ID: {RegionId}", id);
            var region = await repository.GetByIdAsync(id);

            if (region == null)
            {
                _logger.LogWarning("Region not found: {RegionId}", id);
                throw new ResourceNotFoundException("Region", id.ToString());
            }

            return region;
        }
    }

    ====================================================================
    */
}
