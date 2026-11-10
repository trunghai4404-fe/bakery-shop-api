using MediatR;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Repositories;
using SharedKernel.Domain;

namespace Ordering.Application.Features.GetShippingMethods;

public record GetShippingMethodsQuery(bool? OnlyActive = true) : IRequest<Result<List<ShippingMethodDto>>>;

public class GetShippingMethodsQueryHandler(
    IShippingMethodRepository shippingMethodRepository
) : IRequestHandler<GetShippingMethodsQuery, Result<List<ShippingMethodDto>>>
{
    public async Task<Result<List<ShippingMethodDto>>> Handle(GetShippingMethodsQuery request, CancellationToken ct)
    {
        var methods = await shippingMethodRepository.GetAllAsync(request.OnlyActive, ct);
        var dtos = methods.Select(m => new ShippingMethodDto(
            m.Code,
            m.Name,
            m.Fee,
            m.FeeDescription,
            m.EstimatedDeliveryTime,
            m.IsEnabled
        )).ToList();

        return Result.Success(dtos);
    }
}
