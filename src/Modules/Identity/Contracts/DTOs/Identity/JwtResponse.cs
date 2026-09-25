namespace Contract.DTOs.Identity;

public class JwtResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public int AccessTokenExpireAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpireAt { get; set; }
}