namespace Contract.DTOs.Identity;

public class AuthResponseDto
{
    public JwtResponse JwtResponse { get; set; } = null!;
    public UserDto User { get; set; } = null!;
}