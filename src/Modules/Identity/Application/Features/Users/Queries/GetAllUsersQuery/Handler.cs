using Contract.DTOs.Identity;
using Identity.Domain.Enums;
using Identity.Domain.Repository;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Identity.Application.Features.Users.Queries.GetAllUsersQuery;

public class Handler(
    IUserRepository userRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<GetAllUsersQuery, Result<PagedList<ListUsersDto>>>
{
    public async Task<Result<PagedList<ListUsersDto>>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        if (!currentUserService.HasPermission(PermissionEnum.User.UserView))
        {
            return Result.Failure<PagedList<ListUsersDto>>(Error.Forbidden());
        }
        var pagedUsers = await userRepository.GetAllUsersAsync(
            request.Search,
            request.RoleId,
            request.RoleCode,
            request.IsDeleted,
            request.Page,
            request.PageSize,
            ct
        );

        var userDtos = pagedUsers.Items.Select(u => new ListUsersDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            Avatar = u.Avatar,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            IsDeleted = u.IsDeleted,
            RoleCode = u.UserRoles?.Select(ur => ur.Role.Code).ToList() ?? new List<string>()
        }).ToList();

        var response = new PagedList<ListUsersDto>(
            userDtos,
            pagedUsers.Page,
            pagedUsers.PageSize,
            pagedUsers.TotalCount
        );
        return Result.Success(response);
    }
}