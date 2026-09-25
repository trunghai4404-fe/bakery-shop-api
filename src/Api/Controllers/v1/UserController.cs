using Identity.Application.Features.Users.Commands.AssignRoles;
using Identity.Application.Features.Users.Commands.UpdateProfile;
using Identity.Application.Features.Users.Queries.GetAllUsersQuery;
using Identity.Application.Features.Users.Queries.Profile;
using Contract.DTOs.Identity;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/v1/users")]
public class UserController(ISender sender) : ApiControllerBase
{
    // Get me
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var query = new GetProFileQuery();
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    // Update me
    [HttpPatch("me")]
    public async Task<IActionResult> PatchProfile([FromBody] UpdateProfileCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }
    
    // get list user
    [HttpGet]
    [HasPermission(PermissionEnum.User.UserView)]
    public async Task<IActionResult> GetListUser(
        [FromQuery] string? search,
        [FromQuery] int? roleId,
        [FromQuery] string? roleCode,
        [FromQuery] bool? isDeleted,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetAllUsersQuery(
            search,
            roleId,
            roleCode,
            isDeleted,
            page,
            pageSize);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }
    
    // assign roles
    [HttpPut("{userId:guid}/roles")]
    [HasPermission(PermissionEnum.User.UserUpdate)]
    public async Task<IActionResult> UpdateProfile(
        [FromRoute] Guid userId,
        [FromBody] AssignRoleRequest request,
        CancellationToken ct
    )
    {
        var command = new AssignRolesCommand(userId, request.RoleIds, request.RoleCodes);
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }
}