namespace Identity.Domain.Entities;

public class UserRole
{
    public  Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    
    public int RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    
    private UserRole() { }

    public UserRole(Guid userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}