using MediatR;

namespace Contract.DTOs.Identity;

public record AssignRoleRequest(
    List<int>? RoleIds = null,
    List<string>? RoleCodes = null
);