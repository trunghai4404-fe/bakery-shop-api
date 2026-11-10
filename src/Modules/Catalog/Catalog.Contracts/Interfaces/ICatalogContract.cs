using Catalog.Contracts.DTOs.Products;

namespace Catalog.Contracts.Interfaces;

public interface ICatalogContract
{
    Task<List<CatalogVariantValidationDto>> GetVariantsByIdsAsync(IEnumerable<long> variantIds, CancellationToken ct = default);
}
