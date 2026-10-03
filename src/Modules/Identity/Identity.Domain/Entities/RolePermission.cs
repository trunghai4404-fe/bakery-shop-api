namespace Identity.Domain.Entities;

public class RolePermission
{
    public int RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    public int PermissionId { get; private set; }
    public Permission Permission { get; private set; } = null!;

    public RolePermission()
    {
        
    }
    public RolePermission(int roleId, int permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}