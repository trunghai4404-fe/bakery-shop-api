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
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<Ward> Wards => Set<Ward>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContentsDbContext).Assembly);
    }
}
