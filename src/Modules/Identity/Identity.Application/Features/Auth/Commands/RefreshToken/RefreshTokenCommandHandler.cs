using Contract.DTOs.Identity;
using Identity.Domain.Errors;
using Identity.Domain.Repository;
using Identity.Application.Interfaces;
using MediatR;
using SharedKernel.Domain;
using SharedKernel.Domain.Interfaces;

namespace Identity.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IJwtProvider jwtProvider,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
    ) : IRequestHandler<RefreshTokenCommand, Result<JwtResponse>>
{
    public async Task<Result<JwtResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var user = await userRepository.FindUserByRefreshToken(request.RefreshToken, ct);
        if (user == null)
        {
            return Result.Failure<JwtResponse>(IdentityErrors.UserNotFound);
        }

        var roles = user.UserRoles
            .Select(ur => ur.Role.Code)
            .ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .ToList();
        
        var (accessToken, expiresIn) = jwtProvider.GenerateAccessToken(user, roles, permissions);
        user.RevokeRefreshToken();
        var (newRefreshToken, refreshExpiresIn) = jwtProvider.GenerateRefreshToken();
        user.AddRefreshToken(newRefreshToken, refreshExpiresIn);
        
        userRepository.Update(user);
        await  unitOfWork.SaveChangesAsync(ct);

        var response = new JwtResponse
        {
            AccessToken = accessToken,
            AccessTokenExpireAt = expiresIn,
            RefreshToken = newRefreshToken,
            RefreshTokenExpireAt = refreshExpiresIn
        };
        return Result.Success(response);
    }
}