using Catalog.Contracts.DTOs.Products;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Products.Queries.GetProductBySlugQuery;

public record GetProductBySlugQuery(string Slug) : IRequest<Result<ProductDetailDto>>;
