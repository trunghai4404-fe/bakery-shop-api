using SharedKernel.Abstractions;
using SharedKernel.Domain.Errors;

namespace Shared.Web;

using SharedKernel.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _sender;
    private ICurrentUserService? _currentUserService;

    /// <summary>
    /// ISender MediatR được tự động Inject từ HttpContext ServiceProvider
    /// </summary>
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// ICurrentUserService được tự động Inject từ HttpContext ServiceProvider
    /// </summary>
    protected ICurrentUserService CurrentUser => _currentUserService ??= HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

    /// <summary>
    /// Lấy UserId của người dùng hiện tại thông qua CurrentUserService (Guid?)
    /// </summary>
    protected Guid? GetUserId() => CurrentUser.UserId;

    /// <summary>
    /// Kiểm tra người dùng hiện tại có vai trò Admin hoặc quyền liên quan
    /// </summary>
    protected bool IsAdminUser() => CurrentUser.IsAdmin;

    /// <summary>
    /// Xử lý Result có dữ liệu trả về (Result<T>)
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result, string successMessage = "Success.")
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.SuccessResponse(result.Value, successMessage, StatusCodes.Status200OK));
        }

        return MapErrorToResponse<T>(result.Error);
    }

    /// <summary>
    /// Xử lý Result không có dữ liệu trả về (Result void)
    /// </summary>
    protected IActionResult HandleResult(Result result, string successMessage = "Success.")
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse.SuccessResponse(successMessage, StatusCodes.Status200OK));
        }

        return MapErrorToResponse<object>(result.Error);
    }

    /// <summary>
    /// Ánh xạ tự động từ ErrorType sang HTTP Status Code tương ứng
    /// </summary>
    private IActionResult MapErrorToResponse<T>(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

        var response = ApiResponse<T>.FailureResponse(error, statusCode);
        return StatusCode(statusCode, response);
    }
}
