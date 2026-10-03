namespace Contract.DTOs.Roles;

public class RoleDetailResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; } = string.Empty;
    
    public ICollection<PermissionsDto>? Permissions { get; set; } = new List<PermissionsDto>();
}