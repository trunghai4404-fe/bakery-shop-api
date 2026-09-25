using Contract.DTOs.Roles;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Identity.Application.Features.Roles.Queries.GetAllRoles;

public record GetListRolesQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<ListRolesDto>>>;