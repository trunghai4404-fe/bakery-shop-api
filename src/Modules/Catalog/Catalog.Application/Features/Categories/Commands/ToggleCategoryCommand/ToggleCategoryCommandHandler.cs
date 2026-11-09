using Catalog.Contracts.DTOs.Categories;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Commands.ToggleCategoryCommand;

public class ToggleCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork
) : IRequestHandler<ToggleCategoryCommand, Result<ToggleCategoryResponse>>
{
    public async Task<Result<ToggleCategoryResponse>> Handle(
        ToggleCategoryCommand request,
        CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, ct);
        if (category is null)
        {
            return Result.Failure<ToggleCategoryResponse>(CatalogErrors.CategoryNotFound);
        }

        if (request.IsActive.HasValue)
        {
            category.SetActive(request.IsActive.Value);
        }
        else
        {
            category.ToggleStatus();
        }

        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(ct);

        var response = new ToggleCategoryResponse
        {
            Id = category.Id,
            IsActive = category.IsActive
        };

        return Result.Success(response);
    }
}
