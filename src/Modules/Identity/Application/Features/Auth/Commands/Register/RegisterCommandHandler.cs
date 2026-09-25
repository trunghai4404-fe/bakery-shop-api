using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Errors;
using Identity.Domain.Repository;
using Identity.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain;
using SharedKernel.Domain.Interfaces;

namespace Identity.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IRoleRepository roleRepository
    ) : IRequestHandler<RegisterCommand, Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken ct)
    {
        var isEmailExits = await userRepository.IsEmailExists(request.Email, ct);
        if (isEmailExits)
        {
            return Result.Failure(IdentityErrors.EmailAlreadyExists);
        }
        
        var passWordHash = passwordHasher.HashPassword(request.Password);
        
        var userResult = User.Create(request.FullName, request.Email, passWordHash);
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error);
        }

        var user = userResult.Value;
        var defaultRole = await roleRepository.GetByCodeAsync(RoleEnum.User, ct);
        if (defaultRole != null)
        {
            user.UserRoles.Add(new UserRole(user.Id, defaultRole.Id));
        }

        
        await userRepository.AddAsync(user, ct);

        try
        {
            await unitOfWork.SaveChangesAsync(ct);

        }
        catch (DbUpdateException)
        {
            if (await userRepository.IsEmailExists(request.Email, ct))
            {
                return Result.Failure(IdentityErrors.EmailAlreadyExists);
            }

            throw;
        }
        return Result.Success();
    }
}