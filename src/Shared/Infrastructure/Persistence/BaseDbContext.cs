using SharedKernel.Abstractions;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Interfaces;

namespace Shared.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

public abstract class BaseDbContext(
    DbContextOptions options,
    ICurrentUserService? currentUserService = null) : DbContext(options), IUnitOfWork
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserService?.UserId?.ToString()
                            ?? currentUserService?.UserName
                            ?? "Admin System";
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = currentUserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedAt = DateTime.UtcNow;
                entry.Entity.LastModifiedBy = currentUserId;
            }
        }

        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}