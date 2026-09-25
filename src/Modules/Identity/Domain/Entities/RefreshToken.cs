using SharedKernel.Domain.Entities;

namespace Identity.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; } 
    
    public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;
    public DateTime ExpiresOn { get; private set; }
    public DateTime? RevokedOn { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public bool IsActive => RevokedOn == null && !IsExpired;

    public virtual User User { get;private set; } = null!;
    
    private  RefreshToken()
    {
    }
    public RefreshToken(string token, Guid userId, DateTime expiresOn)
    {
        Token = token;
        UserId = userId;
        ExpiresOn = expiresOn;
        CreatedOn = DateTime.UtcNow;
    }

    public static RefreshToken Create(string token, Guid userId, DateTime expiresOn)
    {
        return new RefreshToken(token, userId, expiresOn);
    }

    public void Revoke()
    {
        RevokedOn = DateTime.UtcNow;
    }
}