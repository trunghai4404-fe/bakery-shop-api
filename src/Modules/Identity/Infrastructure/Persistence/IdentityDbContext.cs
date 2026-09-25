using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistence;
using SharedKernel.Abstractions;

namespace Identity.Infrastructure.Persistence;

public class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    ICurrentUserService? currentUserService = null) : BaseDbContext(options, currentUserService)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public  DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken>  RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}