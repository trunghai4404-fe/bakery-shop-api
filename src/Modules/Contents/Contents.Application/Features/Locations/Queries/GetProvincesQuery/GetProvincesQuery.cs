using Contents.Contracts.DTOs.Locations;
using Contents.Contracts.Interfaces;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Locations.Queries.GetProvincesQuery;

public record GetProvincesQuery : IRequest<Result<List<ProvinceDto>>>;

public class GetProvincesQueryHandler(
    ILocationService locationService
) : IRequestHandler<GetProvincesQuery, Result<List<ProvinceDto>>>
{
    public async Task<Result<List<ProvinceDto>>> Handle(
        GetProvincesQuery request, 
        CancellationToken ct)
    {
        var provinces = await locationService.GetProvincesAsync(ct);
        return Result.Success(provinces);
    }
}
