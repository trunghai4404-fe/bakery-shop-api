using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Domain.Entities;
using Identity.Application.Interfaces;
using Infrastructure.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using SharedKernel.Abstractions;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Identity.Infrastructure.Persistence.Services;

public class JwtProvider(IOptions<JwtOptions> options, IDateTimeProvider dateTimeProvider) : IJwtProvider
{
    private readonly JwtOptions jwtOptions = options.Value;
    public (string AccessToken, int ExpiresIn) GenerateAccessToken(
        User user, 
        IEnumerable<string> roles,
        IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Select(p => new Claim("permission", p)));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpiresIn),
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            SigningCredentials = credentials
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessTokenString = tokenHandler.WriteToken(token);
        return (accessTokenString, jwtOptions.AccessTokenExpiresIn * 60);
    }

    public (string RefreshToken, DateTime ExpiresIn) GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        var refreshToken = Convert.ToBase64String(randomBytes);
        DateTime expiresIn = dateTimeProvider.UtcNow.AddDays(7);
        return (refreshToken, expiresIn);

    }

}