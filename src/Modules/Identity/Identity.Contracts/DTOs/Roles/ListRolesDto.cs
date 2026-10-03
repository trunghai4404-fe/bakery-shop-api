namespace Contract.DTOs.Roles;

public class ListRolesDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Permissions { get; set; } = new List<string>();
}