namespace Contents.Contracts.DTOs.Stores;

public record StoreDto(
    long Id,
    string Name,
    string Address,
    int ProvinceId,
    int WardId,
    double Latitude,
    double Longitude,
    string? Phone,
    string? Email,
    string? OpeningHours,
    bool IsActive,
    int SortOrder,
    DateTime CreatedAt,
    DateTime? LastModifiedAt
);

public record StoreSnapshotDto(
    long Id,
    string Name,
    string Address,
    int ProvinceId,
    int WardId,
    double Latitude,
    double Longitude,
    string? Phone,
    string? Email,
    string? OpeningHours
);

public record StoreRequest(
    string Name,
    string Address,
    int ProvinceId,
    int WardId,
    double Latitude,
    double Longitude,
    string? Phone = null,
    string? Email = null,
    string? OpeningHours = null,
    bool IsActive = true,
    int SortOrder = 0
);

public record ToggleStoreRequest(bool? IsActive = null);
public record ToggleStoreResponse(long Id, bool IsActive);
