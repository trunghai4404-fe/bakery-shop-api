using Contract.DTOs.Roles;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Errors;
using Identity.Domain.Repository;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Interfaces;

namespace Identity.Application.Features.Roles.Commands.CreateRole;

public class Handler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService
    ) : IRequestHandler<CreateRoleCommand, Result<CreateResponse>>
{
    public async Task<Result<CreateResponse>> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var roleCode = request.Code.Trim().ToUpper();
        
        var hasPermission = currentUserService.HasPermission(PermissionEnum.Role.RoleCreate);
        if (!hasPermission)
        {
            return Result.Failure<CreateResponse>(Error.Forbidden());
        }
        
        var existingRole = await roleRepository.GetByCodeAsync(request.Code, ct);
        if (existingRole != null)
        {
            return Result.Failure<CreateResponse>(IdentityErrors.RoleAlreadyExists);
        }

        var role = new Role(
            roleCode,
            request.Name.Trim(),
            request.Description?.Trim(),
            isSystemRole: false
        );

        if (request.PermissionIds != null && request.PermissionIds.Count > 0)
        {
            foreach (var permissionId in request.PermissionIds.Distinct())
            {
                role.RolePermissions.Add(new RolePermission(role.Id, permissionId));
            }
        }


        await roleRepository.CreateAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var response = new CreateResponse
        {
            RoleId = role.Id
        };
        return Result.Success(response);

    }
}