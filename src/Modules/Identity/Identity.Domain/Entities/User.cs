using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Interfaces;

namespace Identity.Domain.Entities;

public class User : AuditableEntity, ISoftDelete
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber {get; private set;} = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool? Sex { get; private set; }
    public string? Avatar { get; private set; } = string.Empty;
    public DateTime? DateOfBirth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    
    public  ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    private User() { }

    private User(string fullName, string email, string passwordHash)
    {
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<User> Create(string fullName, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result.Failure<User>(
                Error.Validation(ErrorCode.InvalidValue, "FullName can't be null or empty"));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<User>(
                Error.Validation(ErrorCode.InvalidValue, "Email can't be null or empty"));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result.Failure<User>(
                Error.Validation(ErrorCode.InvalidValue, "Password can't be null or empty"));
        }

        var user = new User(fullName, email, passwordHash);
        return  Result.Success<User>(user);
    }

    public void Update(string? fullName,string? phoneNumber, bool? sex, string? avatar, DateTime? dateOfBirth)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            FullName = fullName;
        }

        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            PhoneNumber = phoneNumber;
        }
        if(sex.HasValue)
        {
            Sex = sex.Value;
        }

        if (!string.IsNullOrWhiteSpace(avatar))
        {
            Avatar = avatar;
        }

        if (dateOfBirth.HasValue)
        {
            DateOfBirth = dateOfBirth.Value;
        }
    }

    public RefreshToken AddRefreshToken(string token, DateTime expiresIn)
    {
        var refreshToken = RefreshToken.Create(token, Id, expiresIn);
        RefreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public void RevokeRefreshToken()
    {
        foreach (var token in RefreshTokens.Where(t => t.IsActive))
        {
            token.Revoke();
        }
    }
}