using Contents.Application.Features.Contacts;
using Contents.Contracts.DTOs.Contacts;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;

[Route("api/v1/contacts")]
[Authorize]
public class ContactController(ISender sender) : ApiControllerBase
{
    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActive(CancellationToken ct) =>
        HandleResult(await sender.Send(new GetActiveContactsQuery(), ct));

    [HttpGet]
    [HasPermission(PermissionEnum.Contact.ContactView)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? type, [FromQuery] bool? isActive,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) =>
        HandleResult(await sender.Send(new GetContactsQuery(type, isActive, page, pageSize), ct));

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionEnum.Contact.ContactView)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await sender.Send(new GetContactByIdQuery(id), ct));

    [HttpPost]
    [HasPermission(PermissionEnum.Contact.ContactCreate)]
    public async Task<IActionResult> Create([FromBody] ContactRequest request, CancellationToken ct) =>
        HandleResult(await sender.Send(new CreateContactCommand(request), ct));

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionEnum.Contact.ContactUpdate)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContactRequest request, CancellationToken ct) =>
        HandleResult(await sender.Send(new UpdateContactCommand(id, request), ct));

    [HttpPatch("{id:guid}/toggle")]
    [HasPermission(PermissionEnum.Contact.ContactUpdate)]
    public async Task<IActionResult> Toggle(Guid id, [FromBody] ToggleContactRequest? request, CancellationToken ct) =>
        HandleResult(await sender.Send(new ToggleContactCommand(id, request?.IsActive), ct));

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionEnum.Contact.ContactDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        HandleResult(await sender.Send(new DeleteContactCommand(id), ct), "Contact deleted successfully.");
}
