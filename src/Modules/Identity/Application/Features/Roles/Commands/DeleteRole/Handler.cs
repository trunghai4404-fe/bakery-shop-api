using Identity.Domain.Enums;
using Identity.Domain.Errors;
using Identity.Domain.Repository;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Interfaces;

namespace Identity.Application.Features.Roles.Commands.DeleteRole;

public class Handler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService
) : IRequestHandler<DeleteRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken ct)
    {
        var hasPermission = currentUserService.HasPermission(PermissionEnum.Role.RoleDelete);
        if (!hasPermission)
        {
            return Result.Failure(Error.Forbidden());
        }
        
        var role = await roleRepository.GetByIdAsync(request.RoleId, ct);
        if (role is null)
        {
            return Result.Failure<bool>(IdentityErrors.RoleNotFound);
        }

        roleRepository.DeleteAsync(role);
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}