using MediatR;
using SharedKernel.Domain;

namespace Identity.Application.Features.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(
    int RoleId
) : IRequest<Result>;