using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Shared.Web;

public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) 
        : base(options)
    {
    }
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // 1. Kiểm tra xem Policy mặc định đã có chưa
        var policy = await base.GetPolicyAsync(policyName);
        if (policy != null)
        {
            return policy;
        }
        // 2. Nếu chưa có, TỰ ĐỘNG TẠO MỚI một Policy cho Permission tên là `policyName`
        return new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
    }
}