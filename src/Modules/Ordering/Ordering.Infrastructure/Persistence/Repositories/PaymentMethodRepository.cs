using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;

namespace Ordering.Infrastructure.Persistence.Repositories;

public class PaymentMethodRepository(OrderingDbContext dbContext) : IPaymentMethodRepository
{
    public async Task<List<PaymentMethod>> GetAllAsync(bool? onlyActive = null, CancellationToken ct = default)
    {
        var query = dbContext.PaymentMethods.AsQueryable();
        if (onlyActive == true)
        {
            query = query.Where(p => p.IsEnabled);
        }
        return await query.OrderBy(p => p.DisplayOrder).ToListAsync(ct);
    }

    public async Task<List<PaymentMethod>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await GetAllAsync(true, ct);
    }

    public async Task<PaymentMethod?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await dbContext.PaymentMethods
            .FirstOrDefaultAsync(p => p.Code.ToUpper() == code.ToUpper(), ct);
    }

    public async Task<PaymentMethod?> GetByIdOrCodeAsync(string idOrCode, CancellationToken ct = default)
    {
        if (int.TryParse(idOrCode, out var id))
        {
            var byId = await dbContext.PaymentMethods.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (byId != null) return byId;
        }

        return await GetByCodeAsync(idOrCode, ct);
    }

    public void Update(PaymentMethod paymentMethod)
    {
        dbContext.PaymentMethods.Update(paymentMethod);
    }
}
