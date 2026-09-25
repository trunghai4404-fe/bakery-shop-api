namespace Contract.DTOs.Roles;
public record UpdateRoleRequest(
    string Name,
    string? Description,
    List<int>? PermissionIds = null,
    List<string>? PermissionCodes = null
);