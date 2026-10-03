using Contract.DTOs.Identity;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Identity.Application.Features.Users.Queries.GetAllUsersQuery;

public record GetAllUsersQuery(
    string? Search,
    int? RoleId,
    string? RoleCode,
    bool? IsDeleted,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<ListUsersDto>>>;