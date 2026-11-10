using MediatR;
using Ordering.Application.Features.CreateOrderFromBasket;
using Ordering.Contracts.DTOs;
using Ordering.Contracts.Interfaces;
using SharedKernel.Domain;

namespace Ordering.Infrastructure.Services;

public class OrderingContract(ISender sender) : IOrderingContract
{
    public async Task<Result<CreateOrderResultDto>> CreateOrderFromBasketAsync(CreateOrderFromBasketRequest request, CancellationToken ct = default)
    {
        return await sender.Send(new CreateOrderFromBasketCommand(request), ct);
    }
}
