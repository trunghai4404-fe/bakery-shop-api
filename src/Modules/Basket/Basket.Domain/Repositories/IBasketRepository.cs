using BasketEntity = Basket.Domain.Entities.Basket;

namespace Basket.Domain.Repositories;

public interface IBasketRepository
{
    Task<BasketEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BasketEntity?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<BasketEntity?> GetActiveBySessionIdAsync(string sessionId, CancellationToken ct = default);
    Task AddAsync(BasketEntity basket, CancellationToken ct = default);
    void Update(BasketEntity basket);
}
