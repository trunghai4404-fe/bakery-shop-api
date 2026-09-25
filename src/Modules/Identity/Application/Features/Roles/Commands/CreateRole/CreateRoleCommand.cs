using Contract.DTOs.Roles;
using MediatR;
using SharedKernel.Domain;

namespace Identity.Application.Features.Roles.Commands.CreateRole;

public record CreateRoleCommand(
    string Name,
    string Description,
    string Code,
    List<int>?  PermissionIds) : IRequest<Result<CreateResponse>>;