using Catalog.Contracts.DTOs.Products;
using Catalog.Contracts.Interfaces;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Services;

public class CatalogContract(CatalogDbContext dbContext) : ICatalogContract
{
    public async Task<List<CatalogVariantValidationDto>> GetVariantsByIdsAsync(IEnumerable<long> variantIds, CancellationToken ct = default)
    {
        var ids = variantIds.Distinct().ToList();
        if (ids.Count == 0) return [];

        return await dbContext.ProductVariants
            .AsNoTracking()
            .Include(v => v.Product)
                .ThenInclude(p => p.ProductImages)
            .Where(v => ids.Contains(v.Id))
            .Select(v => new CatalogVariantValidationDto(
                v.Id,
                v.ProductId,
                v.Product.Name,
                v.Name,
                v.Sku,
                v.Price,
                v.StockQuantity,
                v.IsActive && v.Product.IsActive,
                v.Product.ProductImages
                    .OrderByDescending(img => img.IsPrimary)
                    .ThenBy(img => img.SortOrder)
                    .Select(img => img.DesktopUrl)
                    .FirstOrDefault()
            ))
            .ToListAsync(ct);
    }
}
