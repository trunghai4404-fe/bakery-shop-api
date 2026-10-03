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

namespace Identity.Application.Features.Roles.Commands.UpdateRole;

public class Handler(
    ICurrentUserService currentUserService,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IPermissionRepository permissionRepository
) : IRequestHandler<UpdateRoleCommand, Result<CreateResponse>>
{
    public async Task<Result<CreateResponse>> Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        var hasPermission = currentUserService.HasPermission(PermissionEnum.Role.RoleUpdate);
        if (!hasPermission)
        {
            return Result.Failure<CreateResponse>(Error.Forbidden());
        }
        
        var role =  await roleRepository.GetByIdWithPermissionsAsync(request.Id, ct);
        if (role == null)
        {
            return Result.Failure<CreateResponse>(IdentityErrors.RoleNotFound);
        }
        role.UpdateRole(request.Name, request.Description);
        roleRepository.UpdateAsync(role);

        if (request.PermissionIds != null || request.PermissionCodes != null)
        {
            var targetPermissionIds = new List<int>();
            if (request.PermissionIds != null)
            {
                targetPermissionIds.AddRange(request.PermissionIds);
            }

            if (request.PermissionCodes != null)
            {
                var idsFromCode = await permissionRepository.GetIdsByCodesAsync(request.PermissionCodes, ct);
                targetPermissionIds.AddRange(idsFromCode);
            }
            targetPermissionIds = targetPermissionIds.Distinct().ToList();
            
            role.RolePermissions.Clear();
            foreach (var id in targetPermissionIds)
            {
                role.RolePermissions.Add(new RolePermission(role.Id, id));
            }
        }
        await unitOfWork.SaveChangesAsync(ct);
        var response = new CreateResponse
        {
            RoleId = role.Id,
        };
        return Result.Success(response);
    }
}