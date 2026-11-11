using Ordering.Domain.Entities;
using Ordering.Domain.Enums;

namespace Ordering.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Order?> GetByCodeAsync(string orderCode, CancellationToken ct = default);
    Task<Order?> GetByCodeAndUserIdAsync(string orderCode, Guid userId, CancellationToken ct = default);
    Task<Order?> GetByCodeAndPhoneAsync(string orderCode, string phoneNumber, CancellationToken ct = default);
    Task<(List<Order> Items, int TotalCount)> GetByUserIdPagedAsync(Guid userId, string? keyword, OrderStatus? status, int page, int pageSize, CancellationToken ct = default);
    Task<Dictionary<OrderStatus, int>> GetStatusCountsAsync(Guid? userId, string? keyword, CancellationToken ct = default);
    Task<Dictionary<OrderStatus, int>> GetStatusCountsByUserIdAsync(Guid userId, string? keyword, CancellationToken ct = default);
    Task<(List<Order> Items, int TotalCount)> GetAdminPagedAsync(string? keyword, OrderStatus? status, PaymentStatus? paymentStatus, string? fulfillmentType, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken ct = default);
    Task<(List<Order> Items, int TotalCount)> GetOrdersPagedAsync(Guid? userId, string? keyword, OrderStatus? status, PaymentStatus? paymentStatus, string? fulfillmentType, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken ct = default);
    Task<(Dictionary<OrderStatus, int> StatusCounts, decimal TodayRevenue, decimal MonthRevenue, int TotalOrders)> GetAdminStatisticsAsync(CancellationToken ct = default);
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
