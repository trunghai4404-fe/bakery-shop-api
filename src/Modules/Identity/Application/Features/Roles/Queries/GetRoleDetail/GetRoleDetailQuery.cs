using Contract.DTOs.Roles;
using MediatR;
using SharedKernel.Domain;

namespace Identity.Application.Features.Roles.Queries.GetRoleDetail;

public record GetRoleDetailQuery(int Id) : IRequest<Result<RoleDetailResponse>>;