using Basket.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using BasketEntity = Basket.Domain.Entities.Basket;

namespace Basket.Infrastructure.Persistence.Repositories;

public class BasketRepository(BasketDbContext dbContext) : IBasketRepository
{
    public async Task<BasketEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Baskets
            .FirstOrDefaultAsync(b => b.Id == id && b.IsActive, ct);
    }

    public async Task<BasketEntity?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await dbContext.Baskets
            .FirstOrDefaultAsync(b => b.UserId == userId && b.IsActive, ct);
    }

    public async Task<BasketEntity?> GetActiveBySessionIdAsync(string sessionId, CancellationToken ct = default)
    {
        return await dbContext.Baskets
            .FirstOrDefaultAsync(b => b.SessionId == sessionId && b.IsActive, ct);
    }

    public async Task AddAsync(BasketEntity basket, CancellationToken ct = default)
    {
        await dbContext.Baskets.AddAsync(basket, ct);
    }

    public void Update(BasketEntity basket)
    {
        dbContext.Baskets.Update(basket);
    }
}
