using Contents.Contracts.DTOs.Stores;
using Contents.Domain.Repositories;
using Contents.Domain.Errors;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Contents.Application.Features.Stores;

public record GetStoresQuery(
    string? Search,
    int? ProvinceId,
    int? WardId,
    bool? IsActive,
    double? Latitude,
    double? Longitude,
    double? RadiusKm,
    string? SortBy,
    bool IsDescending,
    int Page,
    int PageSize,
    bool ActiveOnly = false
) : IRequest<Result<PagedList<StoreDto>>>;
public record GetStoreByIdQuery(long Id) : IRequest<Result<StoreDto>>;

public class GetStoresQueryHandler(IStoreRepository stores)
    : IRequestHandler<GetStoresQuery, Result<PagedList<StoreDto>>>
{
    public async Task<Result<PagedList<StoreDto>>> Handle(GetStoresQuery request, CancellationToken ct)
    {
        if (request.Latitude.HasValue != request.Longitude.HasValue ||
            request.Latitude is < -90 or > 90 || request.Longitude is < -180 or > 180 ||
            request.RadiusKm is <= 0 || (request.RadiusKm.HasValue && !request.Latitude.HasValue))
            return Result.Failure<PagedList<StoreDto>>(Error.Validation("INVALID_STORE_SEARCH", "Latitude and longitude must be provided together with valid coordinates; radiusKm must be positive."));

        var results = await stores.GetStoresAsync(
            request.Search,
            request.ProvinceId,
            request.WardId,
            request.IsActive,
            request.Latitude,
            request.Longitude,
            request.SortBy,
            request.IsDescending,
            request.Page,
            request.PageSize,
            request.RadiusKm,
            request.ActiveOnly,
            ct
        );
        var response = new PagedList<StoreDto>(
            results.Items.Select(CreateStoreCommandHandler.ToDto).ToList(),
            results.Page,
            results.PageSize,
            results.TotalCount
        );
        return Result.Success(response);
    }
}

public class GetStoreByIdQueryHandler(IStoreRepository stores)
    : IRequestHandler<GetStoreByIdQuery, Result<StoreDto>>
{
    public async Task<Result<StoreDto>> Handle(GetStoreByIdQuery request, CancellationToken ct)
    {
        var store = await stores.GetByIdAsync(request.Id, ct);
        return store == null
            ? Result.Failure<StoreDto>(ContentsErrors.StoreNotFound)
            : Result.Success(CreateStoreCommandHandler.ToDto(store));
    }
}
