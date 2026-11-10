using Basket.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistence;
using SharedKernel.Abstractions;

namespace Basket.Infrastructure.Persistence;

public class BasketDbContext(
    DbContextOptions<BasketDbContext> options,
    ICurrentUserService? currentUserService = null) : BaseDbContext(options, currentUserService), IBasketUnitOfWork
{
    public DbSet<Basket.Domain.Entities.Basket> Baskets => Set<Basket.Domain.Entities.Basket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BasketDbContext).Assembly);
    }
}
