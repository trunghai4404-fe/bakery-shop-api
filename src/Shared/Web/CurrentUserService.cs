using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SharedKernel.Abstractions;

namespace Shared.Web;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var userIdClaims = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User?.FindFirst("sub")?.Value;
            return Guid.TryParse(userIdClaims, out var userId) ? userId : null;
        }
    }

    public string? UserName => User?.FindFirst(ClaimTypes.Name)?.Value;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    public bool IsAdmin => User?.IsInRole("Admin") ?? false;
    public bool HasPermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission) || !IsAuthenticated)
            return false;
        return User?.Claims
            .Where(c => c.Type == "permission")
            .Any(c => c.Value == permission) ?? false;
    }
}