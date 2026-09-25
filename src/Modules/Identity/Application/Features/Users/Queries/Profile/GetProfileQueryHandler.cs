using Contract.DTOs;
using Contract.DTOs.Identity;
using Identity.Domain.Errors;
using Identity.Domain.Repository;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Domain;

namespace Identity.Application.Features.Users.Queries.Profile;

public class GetProfileQueryHandler(
    ICurrentUserService currentUserService,
    IUserRepository userRepository
    ) : IRequestHandler<GetProFileQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetProFileQuery request, CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            return Result.Failure<UserDto>(IdentityErrors.UserNotFound);
        }

        var user = await userRepository.GetUserById(userId.Value, ct);
        if (user == null)
        {
            return Result.Failure<UserDto>(IdentityErrors.UserNotFound);
        }

        var roles = user.UserRoles
            .Select(ur => ur.Role.Code)
            .ToList();

        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();
        var response = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Avatar = user.Avatar,
            IsActive = user.IsActive,
            Sex = user.Sex,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.LastModifiedAt,
            IsDeleted = user.IsDeleted,
            DeletedAt = user.DeletedAt,
            Roles = roles,
            Permissions = permissions
        };

        return Result.Success(response);
    }
}