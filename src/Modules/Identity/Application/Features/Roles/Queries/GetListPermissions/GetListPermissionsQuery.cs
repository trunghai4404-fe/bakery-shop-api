using Contract.DTOs.Roles;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Identity.Application.Features.Roles.Queries.GetListPermissions;

public record GetListPermissionsQuery(
    string? Search = null,
    int? RoleId = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<PermissionsDto>>>;