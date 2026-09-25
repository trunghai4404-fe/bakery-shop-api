using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IJwtProvider
{
    (string AccessToken, int ExpiresIn) GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);
    (string RefreshToken, DateTime ExpiresIn) GenerateRefreshToken();
}