using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Errors;
using Identity.Domain.Repository;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Interfaces;

namespace Identity.Application.Features.Users.Commands.AssignRoles;

public class AssignRolesHandler(
    IRoleRepository roleRepository,
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork ) : IRequestHandler<AssignRolesCommand, Result>
{
    public async Task<Result> Handle(AssignRolesCommand request, CancellationToken ct)
    {
        if (!currentUserService.HasPermission(PermissionEnum.User.UserUpdate))
        {
            return Result.Failure(Error.Forbidden());
        }

        var user = await userRepository.GetUserById(request.UserId, ct);
        if (user == null)
        {
            return Result.Failure(IdentityErrors.UserNotFound);
        }

        var targetRoleIds = new List<int>();
        if (request.RoleIds != null && request.RoleIds.Any())
        {
            targetRoleIds.AddRange(request.RoleIds);
        }

        if (request.RoleCodes != null && request.RoleCodes.Any())
        {
            var idsByCode = await roleRepository.GetIdsByCodeAsync(request.RoleCodes, ct);
            targetRoleIds.AddRange(idsByCode);
        }

        targetRoleIds = targetRoleIds.Distinct().ToList();
        user.UserRoles.Clear();
        
        foreach (var id in targetRoleIds)
        {
            user.UserRoles.Add(new UserRole(user.Id, id));
        }
        
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}