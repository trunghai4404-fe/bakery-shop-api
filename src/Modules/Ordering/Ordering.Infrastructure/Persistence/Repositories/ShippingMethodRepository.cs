using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;

namespace Ordering.Infrastructure.Persistence.Repositories;

public class ShippingMethodRepository(OrderingDbContext dbContext) : IShippingMethodRepository
{
    public async Task<List<ShippingMethod>> GetAllAsync(bool? onlyActive = null, CancellationToken ct = default)
    {
        var query = dbContext.ShippingMethods.AsQueryable();
        if (onlyActive == true)
        {
            query = query.Where(s => s.IsEnabled);
        }
        return await query.OrderBy(p => p.DisplayOrder).ToListAsync(ct);
    }

    public async Task<List<ShippingMethod>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await GetAllAsync(true, ct);
    }

    public async Task<ShippingMethod?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await dbContext.ShippingMethods
            .FirstOrDefaultAsync(s => s.Code.ToUpper() == code.ToUpper(), ct);
    }

    public async Task<ShippingMethod?> GetByIdOrCodeAsync(string idOrCode, CancellationToken ct = default)
    {
        if (int.TryParse(idOrCode, out var id))
        {
            var byId = await dbContext.ShippingMethods.FirstOrDefaultAsync(s => s.Id == id, ct);
            if (byId != null) return byId;
        }

        return await GetByCodeAsync(idOrCode, ct);
    }

    public void Update(ShippingMethod shippingMethod)
    {
        dbContext.ShippingMethods.Update(shippingMethod);
    }
}
