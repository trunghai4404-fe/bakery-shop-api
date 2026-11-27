using Contents.Contracts.DTOs.Stores;
using Contents.Contracts.Interfaces;
using Contents.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Contents.Infrastructure.Services;

public class StoreContract(ContentsDbContext context) : IStoreContract
{
    public Task<StoreSnapshotDto?> GetActiveStoreAsync(long id, CancellationToken ct = default) =>
        context.Stores.AsNoTracking()
            .Where(s => s.Id == id && s.IsActive)
            .Select(s => new StoreSnapshotDto(
                s.Id,
                s.Name,
                s.Address,
                s.ProvinceId,
                s.WardId,
                s.Latitude,
                s.Longitude,
                s.Phone,
                s.Email,
                s.OpeningHours
            ))
            .FirstOrDefaultAsync(ct);
}
