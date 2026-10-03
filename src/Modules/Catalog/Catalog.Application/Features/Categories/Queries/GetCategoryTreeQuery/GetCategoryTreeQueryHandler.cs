using Catalog.Contracts.DTOs.Categories;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Queries.GetCategoryTreeQuery;

public class GetCategoryTreeQueryHandler(
    ICategoryRepository categoryRepository
) : IRequestHandler<GetCategoryTreeQuery, Result<List<CategoryTreeDto>>>
{
    public async Task<Result<List<CategoryTreeDto>>> Handle(
        GetCategoryTreeQuery request,
        CancellationToken ct)
    {
        var categories = await categoryRepository.GetListAsync(
            isActive: request.OnlyActive == true ? true : null,
            ct: ct
        );

        var dtoMap = categories.ToDictionary(
            c => c.Id,
            c => new CategoryTreeDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ParentId = c.ParentId,
                ImageUrl = c.ImageUrl,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive,
                Children = new List<CategoryTreeDto>()
            });

        var rootNodes = new List<CategoryTreeDto>();

        foreach (var c in categories)
        {
            var currentDto = dtoMap[c.Id];

            if (c.ParentId.HasValue && dtoMap.TryGetValue(c.ParentId.Value, out var parentDto))
            {
                parentDto.Children.Add(currentDto);
            }
            else
            {
                rootNodes.Add(currentDto);
            }
        }

        return Result.Success(rootNodes);
    }
}
