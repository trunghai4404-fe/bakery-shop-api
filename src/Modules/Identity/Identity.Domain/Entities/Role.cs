using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Errors;

namespace Identity.Domain.Entities;

public class Role : AuditableEntity<int>
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public  string? Description { get; private set; }
    public bool IsSystemRole { get; private set; } = false;

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    
    private Role(){ }
    public Role(string code, string name, string? description, bool isSystemRole = false)
    {
        Code = code;
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
    }

    public Result CanDelete()
    {
        if (IsSystemRole)
        {
            return Result.Failure(Error.Conflict(ErrorCode.Conflict, "Can not delete system role" ));
        }
        return  Result.Success();
    }

    public void UpdateRole(string name, string? description)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name.Trim();
        }

        if (description != null)
        {
            Description = description.Trim();
        }
    }
}