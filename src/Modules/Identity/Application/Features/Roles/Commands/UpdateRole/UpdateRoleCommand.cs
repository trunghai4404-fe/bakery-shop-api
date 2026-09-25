using Contract.DTOs.Roles;
using MediatR;
using SharedKernel.Domain;

namespace Identity.Application.Features.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(
    int Id,
    string Name,
    string? Description,
    List<int>? PermissionIds = null,
    List<string>? PermissionCodes = null
) : IRequest<Result<CreateResponse>>;