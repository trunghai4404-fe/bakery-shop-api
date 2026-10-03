using Contract.DTOs;
using Contract.DTOs.Identity;
using MediatR;
using SharedKernel.Domain;

namespace Identity.Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string? FullName,
    string? PhoneNumber,
    bool? Sex,
    string? Avatar,
    DateTime? DateOfBirth
) : IRequest<Result<UserDto>>;