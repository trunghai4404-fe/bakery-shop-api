using Microsoft.AspNetCore.Authorization;

namespace Shared.Web;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(policy: permission)
    {
        
    }
}