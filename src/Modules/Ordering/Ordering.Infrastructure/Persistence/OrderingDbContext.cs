using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;
using Shared.Infrastructure.Persistence;
using SharedKernel.Abstractions;

namespace Ordering.Infrastructure.Persistence;

public class OrderingDbContext(
    DbContextOptions<OrderingDbContext> options,
    ICurrentUserService? currentUserService = null) : BaseDbContext(options, currentUserService), IOrderingUnitOfWork
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<ShippingMethod> ShippingMethods => Set<ShippingMethod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
    }
}
