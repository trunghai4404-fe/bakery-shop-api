using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.Enums;
using Ordering.Domain.Repositories;

namespace Ordering.Infrastructure.Persistence.Repositories;

public class OrderRepository(OrderingDbContext dbContext) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<Order?> GetByCodeAsync(string orderCode, CancellationToken ct = default)
    {
        return await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode, ct);
    }

    public async Task<Order?> GetByCodeAndUserIdAsync(string orderCode, Guid userId, CancellationToken ct = default)
    {
        return await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode && o.UserId == userId, ct);
    }

    public async Task<Order?> GetByCodeAndPhoneAsync(string orderCode, string phoneNumber, CancellationToken ct = default)
    {
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode, ct);

        if (order != null && string.Equals(order.CustomerInfo.Phone, phoneNumber.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return order;
        }

        return null;
    }

    public Task<(List<Order> Items, int TotalCount)> GetByUserIdPagedAsync(
        Guid userId,
        string? keyword,
        OrderStatus? status,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        return GetOrdersPagedAsync(
            userId,
            keyword,
            status,
            paymentStatus: null,
            fulfillmentType: null,
            fromDate: null,
            toDate: null,
            page,
            pageSize,
            ct);
    }

    public async Task<Dictionary<OrderStatus, int>> GetStatusCountsAsync(
        Guid? userId,
        string? keyword,
        CancellationToken ct = default)
    {
        var query = dbContext.Orders.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(o => o.OrderCode.Contains(keyword.Trim()));
        }

        var counts = await query
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, ct);

        return counts;
    }

    public Task<Dictionary<OrderStatus, int>> GetStatusCountsByUserIdAsync(
        Guid userId,
        string? keyword,
        CancellationToken ct = default)
    {
        return GetStatusCountsAsync(userId, keyword, ct);
    }

    public Task<(List<Order> Items, int TotalCount)> GetAdminPagedAsync(
        string? keyword,
        OrderStatus? status,
        PaymentStatus? paymentStatus,
        string? fulfillmentType,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        return GetOrdersPagedAsync(
            userId: null,
            keyword,
            status,
            paymentStatus,
            fulfillmentType,
            fromDate,
            toDate,
            page,
            pageSize,
            ct);
    }

    public async Task<(List<Order> Items, int TotalCount)> GetOrdersPagedAsync(
        Guid? userId,
        string? keyword,
        OrderStatus? status,
        PaymentStatus? paymentStatus,
        string? fulfillmentType,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = dbContext.Orders
            .Include(o => o.Items)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(o => o.OrderCode.Contains(kw));
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (paymentStatus.HasValue)
        {
            query = query.Where(o => o.PaymentStatus == paymentStatus.Value);
        }

        if (!string.IsNullOrWhiteSpace(fulfillmentType))
        {
            query = query.Where(o => o.FulfillmentType.ToLower() == fulfillmentType.Trim().ToLower());
        }

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= toDate.Value);
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<(Dictionary<OrderStatus, int> StatusCounts, decimal TodayRevenue, decimal MonthRevenue, int TotalOrders)> GetAdminStatisticsAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var statusCounts = await dbContext.Orders
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, ct);

        var totalOrders = await dbContext.Orders.CountAsync(ct);

        var todayRevenue = await dbContext.Orders
            .Where(o => o.Status == OrderStatus.Completed && o.CreatedAt >= today)
            .SumAsync(o => o.TotalAmount, ct);

        var monthRevenue = await dbContext.Orders
            .Where(o => o.Status == OrderStatus.Completed && o.CreatedAt >= firstDayOfMonth)
            .SumAsync(o => o.TotalAmount, ct);

        return (statusCounts, todayRevenue, monthRevenue, totalOrders);
    }

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await dbContext.Orders.AddAsync(order, ct);
    }

    public void Update(Order order)
    {
        dbContext.Orders.Update(order);
    }
}
