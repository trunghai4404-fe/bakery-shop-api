namespace Contract.DTOs.Identity;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? Avatar { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool? Sex  { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public  DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public IReadOnlyCollection<string> Roles { get; set; } = new List<string>();
    public IReadOnlyCollection<string> Permissions { get; set; } = new List<string>();
}