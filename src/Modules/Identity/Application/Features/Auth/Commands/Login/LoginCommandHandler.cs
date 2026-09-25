using Contract.DTOs.Identity;
using Identity.Domain.Errors;
using Identity.Domain.Repository;
using Identity.Application.Interfaces;
using MediatR;
using SharedKernel.Domain;
using SharedKernel.Domain.Interfaces;

namespace Identity.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IJwtProvider jwtProvider
) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetUserByEmail(request.Email, ct);
        if (user == null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResponseDto>(IdentityErrors.InvalidCredentials);
        }

        if (user.IsActive == false)
        {
            return Result.Failure<AuthResponseDto>(IdentityErrors.AccountInActive);
        }

        var roles = user.UserRoles
            .Select(ur => ur.Role.Code)
            .ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();

        var (accessToken, expiresIn) = jwtProvider.GenerateAccessToken(user, roles, permissions);
        var (refreshToken, refreshExpiresIn) = jwtProvider.GenerateRefreshToken();

        user.AddRefreshToken(refreshToken, refreshExpiresIn);

        await unitOfWork.SaveChangesAsync(ct);

        var response = new AuthResponseDto
        {
            JwtResponse = new JwtResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpireAt = expiresIn,
                RefreshTokenExpireAt = refreshExpiresIn
            },
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                Roles = roles,
                Permissions = permissions
            }

        };
        return Result.Success(response);

    }
}