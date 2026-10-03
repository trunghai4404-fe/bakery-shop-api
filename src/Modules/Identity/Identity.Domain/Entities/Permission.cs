using SharedKernel.Domain.Entities;

namespace Identity.Domain.Entities;

public class Permission : AuditableEntity<int>
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    
    private Permission() {}
    
    public Permission(string code, string name, string? description)
    {
        Code = code;
        Name = name;
        Description = description;
    }
}