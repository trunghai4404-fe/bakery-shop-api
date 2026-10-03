using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Commands.DeleteCategoryCommand;

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork
) : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(
        DeleteCategoryCommand request,
        CancellationToken ct)
    {
        // 1. Kiểm tra danh mục có tồn tại không
        var category = await categoryRepository.GetByIdAsync(request.Id, ct);
        if (category is null)
        {
            return Result.Failure(CatalogErrors.CategoryNotFound);
        }

        // 2. Đảm bảo không còn sản phẩm nào thuộc danh mục
        var hasProducts = await categoryRepository.HasProductsAsync(request.Id, ct);
        if (hasProducts)
        {
            return Result.Failure(CatalogErrors.CategoryHasProducts);
        }

        // 3. Đảm bảo không còn danh mục con trực thuộc
        var hasChildren = await categoryRepository.HasChildrenAsync(request.Id, ct);
        if (hasChildren)
        {
            return Result.Failure(CatalogErrors.CategoryHasChildren);
        }

        // 4. Thực hiện xóa danh mục
        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
