using Contents.Contracts.DTOs.Locations;
using Contents.Contracts.Interfaces;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Locations.Queries.GetWardsByDistrictQuery;

public record GetWardsByDistrictQuery(int? DistrictId = null, int? ProvinceId = null) : IRequest<Result<List<WardDto>>>;

public class GetWardsByDistrictQueryHandler(
    ILocationService locationService
) : IRequestHandler<GetWardsByDistrictQuery, Result<List<WardDto>>>
{
    public async Task<Result<List<WardDto>>> Handle(
        GetWardsByDistrictQuery request, 
        CancellationToken ct)
    {
        var wards = await locationService.GetWardsAsync(request.DistrictId, request.ProvinceId, ct);
        return Result.Success(wards);
    }
}
