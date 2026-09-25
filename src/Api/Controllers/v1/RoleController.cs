using Identity.Application.Features.Roles.Commands.CreateRole;
using Identity.Application.Features.Roles.Commands.DeleteRole;
using Identity.Application.Features.Roles.Commands.UpdateRole;
using Identity.Application.Features.Roles.Queries.GetAllRoles;
using Identity.Application.Features.Roles.Queries.GetRoleDetail;
using Contract.DTOs.Roles;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;

[Authorize]
[Route("api/v1/roles")]
public class RoleController(ISender sender) : ApiControllerBase
{
    //Create
    [HttpPost]
    [HasPermission(PermissionEnum.Role.RoleCreate)]
    public async Task<IActionResult> Create(CreateRoleCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }
    
    //Get list
    [HttpGet]
    // [HasPermission(PermissionEnum.Role.RoleView)]
    public async Task<IActionResult> Get(
        [FromQuery] string? search, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetListRolesQuery(search, page, pageSize);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    //Get detail
    [HttpGet("{id:int}")]
    [HasPermission(PermissionEnum.Role.RoleView)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var command = new GetRoleDetailQuery(id);
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    //Update
    [HttpPut("{id:int}")]
    [HasPermission(PermissionEnum.Role.RoleUpdate)]
    public async Task<IActionResult> Update( [FromRoute] int id,[FromBody]UpdateRoleRequest request, CancellationToken ct)
    {
        var command = new UpdateRoleCommand(
            id,
            request.Name,
            request.Description,
            request.PermissionIds,
            request.PermissionCodes
        );
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    //Delete
    [HttpDelete("{id:int}")]
    [HasPermission(PermissionEnum.Role.RoleDelete)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var command = new DeleteRoleCommand(id);
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }
    
}