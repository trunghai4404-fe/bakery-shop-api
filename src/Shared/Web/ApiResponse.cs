using SharedKernel.Domain.Errors;

namespace Shared.Web;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<ApiErrorDetail>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success.", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            Errors = null,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> FailureResponse(string message, int statusCode = 400, List<ApiErrorDetail>? errors = null, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Data = default,
            Errors = errors ?? [],
            Timestamp = DateTime.UtcNow,
            TraceId = traceId
        };
    }

    public static ApiResponse<T> FailureResponse(Error error, int statusCode = 400, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = error.Description,
            Data = default,
            Errors = [new ApiErrorDetail(error.Code, error.Description)],
            Timestamp = DateTime.UtcNow,
            TraceId = traceId
        };
    }
}

/// <summary>
/// Non-generic ApiResponse cho các API không trả về payload data (Create/Update/Delete void)
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse SuccessResponse(string message = "Success.", int statusCode = 200)
    {
        return new ApiResponse
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Message = message,
            Data = null,
            Errors = null,
            Timestamp = DateTime.UtcNow
        };
    }

    public static new ApiResponse FailureResponse(string message, int statusCode = 400, List<ApiErrorDetail>? errors = null, string? traceId = null)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Data = null,
            Errors = errors ?? [],
            Timestamp = DateTime.UtcNow,
            TraceId = traceId
        };
    }
}

public record ApiErrorDetail(string Code, string Description, string? FieldName = null);
