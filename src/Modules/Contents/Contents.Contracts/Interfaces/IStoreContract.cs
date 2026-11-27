using Contents.Contracts.DTOs.Stores;

namespace Contents.Contracts.Interfaces;

public interface IStoreContract
{
    Task<StoreSnapshotDto?> GetActiveStoreAsync(long id, CancellationToken ct = default);
}
