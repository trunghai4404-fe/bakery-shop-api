using Identity.Domain.Errors;
using Identity.Domain.Repository;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Interfaces;

namespace Identity.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler(
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
    ) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            return Result.Failure(Error.Unauthorized);
        }
        var user = await userRepository.GetUserWithRefreshTokenById(userId.Value, ct);

        if (user == null)
        {
            return Result.Failure(IdentityErrors.UserNotFound);
        }

        var targetToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);
        if (targetToken != null)
        {
            targetToken.Revoke();
        }
        else
        {
            user.RevokeRefreshToken();
        }
        
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}