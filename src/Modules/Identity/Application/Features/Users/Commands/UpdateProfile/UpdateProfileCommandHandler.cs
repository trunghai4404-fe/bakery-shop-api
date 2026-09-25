using Contract.DTOs.Identity;
using Identity.Domain.Errors;
using Identity.Domain.Repository;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Interfaces;

namespace Identity.Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandler(
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository
    ) : IRequestHandler<UpdateProfileCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            return Result.Failure<UserDto>(Error.Unauthorized);
        }

        var user = await userRepository.GetUserById(userId.Value, ct);
        if (user == null)
        {
            return Result.Failure<UserDto>(IdentityErrors.UserNotFound);
        }
        user.Update(
            request.FullName,
            request.PhoneNumber,
            request.Sex ?? null,
            request.Avatar,
            request.DateOfBirth
            );
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(ct);

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