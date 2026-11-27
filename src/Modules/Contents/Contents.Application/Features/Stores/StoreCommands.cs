using Contents.Contracts.DTOs.Stores;
using Contents.Domain.Entities;
using Contents.Domain.Errors;
using Contents.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Contents.Application.Features.Stores;

public record CreateStoreCommand(StoreRequest Request) : IRequest<Result<StoreDto>>;
public record UpdateStoreCommand(long Id, StoreRequest Request) : IRequest<Result<StoreDto>>;
public record DeleteStoreCommand(long Id) : IRequest<Result>;
public record ToggleStoreCommand(long Id, bool? IsActive) : IRequest<Result<ToggleStoreResponse>>;

public class CreateStoreCommandHandler(IStoreRepository stores, IContentsUnitOfWork unitOfWork)
    : IRequestHandler<CreateStoreCommand, Result<StoreDto>>
{
    public async Task<Result<StoreDto>> Handle(CreateStoreCommand command, CancellationToken ct)
    {
        var invalid = await Validate(command.Request, stores, ct);
        if (invalid != null) return Result.Failure<StoreDto>(invalid);
        var r = command.Request;
        var store = Store.Create(
            r.Name,
            r.Address,
            r.ProvinceId,
            r.WardId,
            r.Latitude,
            r.Longitude,
            r.Phone,
            r.Email,
            r.OpeningHours,
            r.IsActive,
            r.SortOrder
        );
        await stores.AddAsync(store, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ToDto(store));
    }

    internal static async Task<Error?> Validate(StoreRequest r, IStoreRepository stores, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(r.Name) || r.Name.Length > 200 || string.IsNullOrWhiteSpace(r.Address) || r.Address.Length > 500)
            return Error.Validation("INVALID_STORE", "Name (max 200) and address (max 500) are required.");
        if (r.ProvinceId <= 0 || r.WardId <= 0 || r.Latitude is < -90 or > 90 || r.Longitude is < -180 or > 180)
            return Error.Validation("INVALID_STORE_LOCATION", "Province, ward, latitude, or longitude is invalid.");
        if (r.Phone?.Length > 50 || r.Email?.Length > 254 || r.OpeningHours?.Length > 1000)
            return Error.Validation("INVALID_STORE", "Phone, email, or opening hours exceed the allowed length.");
        if (!await stores.ProvinceExistsAsync(r.ProvinceId, ct))
            return Error.Validation("INVALID_PROVINCE", "Province does not exist or is inactive.");
        if (!await stores.WardExistsInProvinceAsync(r.WardId, r.ProvinceId, ct))
            return Error.Validation("INVALID_WARD", "Ward does not belong to the selected province or is inactive.");
        return null;
    }

    internal static StoreDto ToDto(Store s) => new(
        s.Id,
        s.Name,
        s.Address,
        s.ProvinceId,
        s.WardId,
        s.Latitude,
        s.Longitude,
        s.Phone,
        s.Email,
        s.OpeningHours,
        s.IsActive,
        s.SortOrder,
        s.CreatedAt,
        s.LastModifiedAt
    );
}

public class UpdateStoreCommandHandler(IStoreRepository stores, IContentsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateStoreCommand, Result<StoreDto>>
{
    public async Task<Result<StoreDto>> Handle(UpdateStoreCommand command, CancellationToken ct)
    {
        var store = await stores.GetByIdAsync(command.Id, ct);
        if (store == null) return Result.Failure<StoreDto>(ContentsErrors.StoreNotFound);
        var invalid = await CreateStoreCommandHandler.Validate(command.Request, stores, ct);
        if (invalid != null) return Result.Failure<StoreDto>(invalid);
        var r = command.Request;
        store.Update(
            r.Name,
            r.Address,
            r.ProvinceId,
            r.WardId,
            r.Latitude,
            r.Longitude,
            r.Phone,
            r.Email,
            r.OpeningHours,
            r.IsActive,
            r.SortOrder
        );
        stores.Update(store);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(CreateStoreCommandHandler.ToDto(store));
    }
}

public class DeleteStoreCommandHandler(IStoreRepository stores, IContentsUnitOfWork unitOfWork)
    : IRequestHandler<DeleteStoreCommand, Result>
{
    public async Task<Result> Handle(DeleteStoreCommand command, CancellationToken ct)
    {
        var store = await stores.GetByIdAsync(command.Id, ct);
        if (store == null) return Result.Failure(ContentsErrors.StoreNotFound);
        stores.Delete(store);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ToggleStoreCommandHandler(IStoreRepository stores, IContentsUnitOfWork unitOfWork)
    : IRequestHandler<ToggleStoreCommand, Result<ToggleStoreResponse>>
{
    public async Task<Result<ToggleStoreResponse>> Handle(ToggleStoreCommand command, CancellationToken ct)
    {
        var store = await stores.GetByIdAsync(command.Id, ct);
        if (store == null) return Result.Failure<ToggleStoreResponse>(ContentsErrors.StoreNotFound);
        store.ToggleStatus(command.IsActive);
        stores.Update(store);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(new ToggleStoreResponse(store.Id, store.IsActive));
    }
}
