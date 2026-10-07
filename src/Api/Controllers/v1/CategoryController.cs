using Catalog.Application.Features.Categories.Commands.CreateCategoryCommand;
using Catalog.Application.Features.Categories.Commands.DeleteCategoryCommand;
using Catalog.Application.Features.Categories.Commands.ToggleCategoryCommand;
using Catalog.Application.Features.Categories.Commands.UpdateCategoryCommand;
using Catalog.Application.Features.Categories.Queries.GetAllCategoriesQuery;
using Catalog.Application.Features.Categories.Queries.GetCategoryDetailQuery;
using Catalog.Application.Features.Categories.Queries.GetCategoryTreeQuery;
using Catalog.Contracts.DTOs.Categories;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;

[Route("api/v1/categories")]
public class CategoryController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] long? parentId,
        [FromQuery] bool? isRoot,
        [FromQuery] bool? isFeatured,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetAllCategoriesQuery(
            search,
            isActive,
            parentId,
            isRoot,
            isFeatured,
            sortBy,
            isDescending,
            page,
            pageSize);

        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree([FromQuery] bool? onlyActive = true, CancellationToken ct = default)
    {
        var query = new GetCategoryTreeQuery(onlyActive);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken ct)
    {
        var query = new GetCategoryDetailQuery(id);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize]
    [HasPermission(PermissionEnum.Category.CategoryCreate)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize]
    [HasPermission(PermissionEnum.Category.CategoryUpdate)]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(
            id,
            request.Name,
            request.Slug,
            request.Description,
            request.ParentId,
            request.ImageUrl,
            request.SortOrder,
            request.IsFeatured
        );

        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPatch("{id:long}/toggle")]
    [Authorize]
    [HasPermission(PermissionEnum.Category.CategoryUpdate)]
    public async Task<IActionResult> Toggle(
        [FromRoute] long id,
        [FromBody] ToggleCategoryRequest? request,
        CancellationToken ct)
    {
        var command = new ToggleCategoryCommand(id, request?.IsActive);
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    [HasPermission(PermissionEnum.Category.CategoryDelete)]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken ct)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Category deleted successfully.");
    }
}
