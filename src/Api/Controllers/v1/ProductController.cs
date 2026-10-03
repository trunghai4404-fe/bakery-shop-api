using Catalog.Application.Features.Products.Commands.CreateProductCommand;
using Catalog.Application.Features.Products.Commands.ToggleProductCommand;
using Catalog.Application.Features.Products.Commands.UpdateProductCommand;
using Catalog.Application.Features.Products.Queries.GetProductByIdQuery;
using Catalog.Application.Features.Products.Queries.GetProductBySlugQuery;
using Catalog.Application.Features.Products.Queries.GetProductsQuery;
using Catalog.Contracts.DTOs.Products;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;

[Route("api/v1/products")]
public class ProductController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] long? categoryId,
        [FromQuery] string? categorySlug,
        [FromQuery] bool? isActive,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetProductsQuery(
            search,
            categoryId,
            categorySlug,
            isActive,
            sortBy,
            isDescending,
            page,
            pageSize);

        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken ct)
    {
        var query = new GetProductByIdQuery(id);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug([FromRoute] string slug, CancellationToken ct)
    {
        var query = new GetProductBySlugQuery(slug);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize]
    [HasPermission(PermissionEnum.Product.ProductCreate)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize]
    [HasPermission(PermissionEnum.Product.ProductUpdate)]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Sku,
            request.Slug,
            request.Description,
            request.IsActive,
            request.Images,
            request.Variants,
            request.CategoryIds
        );

        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPatch("{id:long}/toggle")]
    [Authorize]
    [HasPermission(PermissionEnum.Product.ProductUpdate)]
    public async Task<IActionResult> Toggle(
        [FromRoute] long id,
        [FromBody] ToggleProductRequest? request,
        CancellationToken ct)
    {
        var command = new ToggleProductCommand(id, request?.IsActive);
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }
}
