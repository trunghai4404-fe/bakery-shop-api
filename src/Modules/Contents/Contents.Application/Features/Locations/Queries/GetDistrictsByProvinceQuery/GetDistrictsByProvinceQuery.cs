using Contents.Contracts.DTOs.Locations;
using Contents.Contracts.Interfaces;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Locations.Queries.GetDistrictsByProvinceQuery;

public record GetDistrictsByProvinceQuery(int? ProvinceId = null) : IRequest<Result<List<DistrictDto>>>;

public class GetDistrictsByProvinceQueryHandler(
    ILocationService locationService
) : IRequestHandler<GetDistrictsByProvinceQuery, Result<List<DistrictDto>>>
{
    public async Task<Result<List<DistrictDto>>> Handle(
        GetDistrictsByProvinceQuery request, 
        CancellationToken ct)
    {
        var districts = await locationService.GetDistrictsAsync(request.ProvinceId, ct);
        return Result.Success(districts);
    }
}
