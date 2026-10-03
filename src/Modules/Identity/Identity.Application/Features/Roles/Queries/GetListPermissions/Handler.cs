using Contract.DTOs.Roles;
using Identity.Domain.Enums;
using Identity.Domain.Repository;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Identity.Application.Features.Roles.Queries.GetListPermissions;

public class Handler(
    ICurrentUserService currentUserService,
    IPermissionRepository permissionRepository
) : IRequestHandler<GetListPermissionsQuery, Result<PagedList<PermissionsDto>>>
{
    public async Task<Result<PagedList<PermissionsDto>>> Handle(GetListPermissionsQuery request, CancellationToken ct)
    {
        if (!currentUserService.HasPermission(PermissionEnum.Permission.PermissionView))
        {
            return Result.Failure<PagedList<PermissionsDto>>(Error.Forbidden());
        }

        var pagedPermissions = await permissionRepository.GetAllAsync(
            request.Search,
            request.RoleId,
            request.Page,
            request.PageSize,
            ct
        );

        var permissionDto = pagedPermissions.Items
            .Select(r => new PermissionsDto
            {
                Id = r.Id,
                Code = r.Code,
                Name = r.Name,
                Description = r.Description,
            }
        ).ToList();

        var response = new PagedList<PermissionsDto>(
            permissionDto,
            pagedPermissions.Page,
            pagedPermissions.PageSize,
            pagedPermissions.TotalCount
        );
        return Result.Success(response);
    }
}