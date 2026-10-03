using Catalog.Contracts.DTOs.Products;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Products.Commands.ToggleProductCommand;

public class ToggleProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork
) : IRequestHandler<ToggleProductCommand, Result<ToggleProductResponse>>
{
    public async Task<Result<ToggleProductResponse>> Handle(
        ToggleProductCommand request,
        CancellationToken ct)
    {
        var product = await productRepository.GetByIdForUpdateAsync(request.Id, ct);
        if (product is null)
        {
            return Result.Failure<ToggleProductResponse>(CatalogErrors.ProductNotFound);
        }

        if (request.IsActive.HasValue)
        {
            product.SetActive(request.IsActive.Value);
        }
        else
        {
            product.ToggleStatus();
        }

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        var response = new ToggleProductResponse
        {
            Id = product.Id,
            IsActive = product.IsActive
        };

        return Result.Success(response);
    }
}
