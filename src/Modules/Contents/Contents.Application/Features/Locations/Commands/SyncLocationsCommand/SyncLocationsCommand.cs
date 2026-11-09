using Contents.Contracts.Interfaces;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Locations.Commands.SyncLocationsCommand;

public record SyncLocationsCommand : IRequest<Result<int>>;

public class SyncLocationsCommandHandler(
    ILocationService locationService
) : IRequestHandler<SyncLocationsCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        SyncLocationsCommand request, 
        CancellationToken ct)
    {
        var count = await locationService.SyncLocationsFromOpenApiAsync(ct);
        return Result.Success(count);
    }
}
