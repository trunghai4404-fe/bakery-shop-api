namespace Contract.DTOs.Roles;

public class RoleDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    
    public ICollection<PermissionsDto>? Permissions { get; set; } = new List<PermissionsDto>();
}