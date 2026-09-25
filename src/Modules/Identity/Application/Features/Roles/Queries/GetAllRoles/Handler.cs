using Contract.DTOs.Roles;
using Identity.Domain.Enums;
using Identity.Domain.Repository;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Identity.Application.Features.Roles.Queries.GetAllRoles;

public class Handler(
    ICurrentUserService currentUserService,
    IRoleRepository roleRepository
) : IRequestHandler<GetListRolesQuery, Result<PagedList<ListRolesDto>>>
{
    public async Task<Result<PagedList<ListRolesDto>>> Handle(GetListRolesQuery request, CancellationToken ct)
    {
        var hasPermission = currentUserService.HasPermission(PermissionEnum.Role.RoleView);
        if (!hasPermission)
        {
            return Result.Failure<PagedList<ListRolesDto>>(Error.Forbidden());
        }

        var pagedRole = await roleRepository.GetAllAsync(
            request.Search,
            request.Page,
            request.PageSize, ct
        );
        

        var roleDtos = pagedRole.Items.Select(r => new ListRolesDto
        {
            Id = r.Id,
            Code = r.Code,
            Name = r.Name,
            Description = r.Description,
            Permissions = r.RolePermissions?
                .Select(rp => rp.Permission.Code)
                .ToList() ?? new List<string>()
        }).ToList();

        var response = new PagedList<ListRolesDto>(
            roleDtos,
            pagedRole.Page,
            pagedRole.PageSize,
            pagedRole.TotalCount
        );

        return Result.Success(response);
    }
}