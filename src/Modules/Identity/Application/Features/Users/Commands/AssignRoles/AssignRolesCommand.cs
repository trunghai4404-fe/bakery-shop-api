using MediatR;
using SharedKernel.Domain;

namespace Identity.Application.Features.Users.Commands.AssignRoles;

public record AssignRolesCommand(
    Guid UserId,
    List<int>? RoleIds = null,
    List<string>? RoleCodes = null
) : IRequest<Result>;