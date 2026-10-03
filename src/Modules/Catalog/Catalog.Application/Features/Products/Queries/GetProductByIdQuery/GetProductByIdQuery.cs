using Catalog.Contracts.DTOs.Products;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Products.Queries.GetProductByIdQuery;

public record GetProductByIdQuery(long Id) : IRequest<Result<ProductDetailDto>>;
