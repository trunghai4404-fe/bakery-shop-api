using Contract.DTOs.Roles;
using Identity.Domain.Enums;
using Identity.Domain.Errors;
using Identity.Domain.Repository;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Identity.Application.Features.Roles.Queries.GetRoleDetail;

public class Handler(
    IRoleRepository roleRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<GetRoleDetailQuery, Result<RoleDetailResponse>>
{
    public async Task<Result<RoleDetailResponse>> Handle(GetRoleDetailQuery request, CancellationToken ct)
    {
        var hasPermission = currentUserService.HasPermission(PermissionEnum.Role.RoleView);
        if (!hasPermission)
        {
            return Result.Failure<RoleDetailResponse>(Error.Forbidden());
        }
        
        var role = await roleRepository.GetByIdWithPermissionsAsync(request.Id, ct);
        if (role is null)
        {
            return Result.Failure<RoleDetailResponse>(IdentityErrors.RoleNotFound);
        }

        var permissions = role.RolePermissions
            .Select(rp => new PermissionsDto
            {
                Id = rp.Permission.Id,
                Code = rp.Permission.Code,
                Name = rp.Permission.Name,
                Description = rp.Permission.Description
            }).ToList();

        var response = new RoleDetailResponse
        {
            Id = role.Id,
            Code = role.Code,
            Name = role.Name,
            Description = role.Description,
            Permissions = permissions,
            CreatedAt = role.CreatedAt,
            CreatedBy = role.CreatedBy ?? string.Empty,
            UpdatedAt = role.LastModifiedAt,
            UpdatedBy = role.LastModifiedBy ?? string.Empty
        };
        return Result.Success(response);
    }
}