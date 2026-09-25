using Microsoft.AspNetCore.Authorization;

namespace Shared.Web;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string permission { get; }
    public PermissionRequirement(string permission)
    {
        this.permission = permission;
    }
}