using Microsoft.AspNetCore.Authorization;

namespace Shared.Web;

public class PermissionAuthorizationHandler :  AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        // 1. Kiểm tra User đã đăng nhập chưa
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }
        // 2. Trích xuất tất cả các Claim có type = "permission" được gán từ JWT Token
        var userPermissions = context.User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value);
        // 3. Nếu User có chứa Permission yêu cầu -> Đánh dấu THÀNH CÔNG (Succeed)
        if (userPermissions.Contains(requirement.permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}