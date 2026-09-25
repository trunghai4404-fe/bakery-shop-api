using Identity.Application.Features.Roles.Queries.GetListPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;

[Authorize]
[Route("api/v1/permissions")]
public class PermissionController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPermissions(
        [FromQuery] string? search,
        [FromQuery] int? roleId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var command = new GetListPermissionsQuery(
            search,
            roleId,
            page,
            pageSize
        );
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }
}