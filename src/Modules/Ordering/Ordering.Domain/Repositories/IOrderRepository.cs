using Ordering.Domain.Entities;

namespace Ordering.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Order?> GetByCodeAsync(string orderCode, CancellationToken ct = default);
    Task<List<Order>> GetByCustomerIdAsync(long customerId, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    void Update(Order order);
}

public interface IPaymentMethodRepository
{
    Task<List<PaymentMethod>> GetAllAsync(bool? onlyActive = null, CancellationToken ct = default);
    Task<List<PaymentMethod>> GetAllActiveAsync(CancellationToken ct = default);
    Task<PaymentMethod?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<PaymentMethod?> GetByIdOrCodeAsync(string idOrCode, CancellationToken ct = default);
    void Update(PaymentMethod paymentMethod);
}

public interface IShippingMethodRepository
{
    Task<List<ShippingMethod>> GetAllAsync(bool? onlyActive = null, CancellationToken ct = default);
    Task<List<ShippingMethod>> GetAllActiveAsync(CancellationToken ct = default);
    Task<ShippingMethod?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<ShippingMethod?> GetByIdOrCodeAsync(string idOrCode, CancellationToken ct = default);
    void Update(ShippingMethod shippingMethod);
}

public interface IOrderingUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
