using Contents.Domain.Entities;
using Contents.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistence;
using SharedKernel.Abstractions;

namespace Contents.Infrastructure.Persistence;

public class ContentsDbContext(
    DbContextOptions<ContentsDbContext> options,
    ICurrentUserService? currentUserService = null) : BaseDbContext(options, currentUserService), IContentsUnitOfWork
{
    public DbSet<Banner> Banners => Set<Banner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContentsDbContext).Assembly);
    }
}
