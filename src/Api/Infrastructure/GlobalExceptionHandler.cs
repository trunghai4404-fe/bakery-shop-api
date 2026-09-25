using Shared.Web;
using SharedKernel.Commons;
using SharedKernel.Exceptions;

namespace Api.Infrastructure;

using Microsoft.AspNetCore.Diagnostics;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        if (exception is ValidationException validationException)
        {
            logger.LogWarning("Validation failed for request. TraceId: {TraceId}, ErrorsCount: {Count}", traceId, validationException.Errors.Count);

            var validationErrors = validationException.Errors
                .Select(e => new ApiErrorDetail(ErrorCode.ValidationError, e.ErrorMessage, e.PropertyName))
                .ToList();

            var validationResponse = ApiResponse.FailureResponse(
                message: "One or more validation errors occurred.",
                statusCode: StatusCodes.Status400BadRequest,
                errors: validationErrors,
                traceId: traceId
            );

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(validationResponse, cancellationToken);
            return true;
        }

        logger.LogError(exception, "Unhandled Exception occurred. TraceId: {TraceId}", traceId);

        var internalErrorResponse = ApiResponse.FailureResponse(
            message: "An internal server error occurred. Please try again later.",
            statusCode: StatusCodes.Status500InternalServerError,
            traceId: traceId
        );

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(internalErrorResponse, cancellationToken);

        return true;
    }
}
