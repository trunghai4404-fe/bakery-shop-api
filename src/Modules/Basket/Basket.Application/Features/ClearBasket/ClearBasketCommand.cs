using Basket.Domain.Errors;
using Basket.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Basket.Application.Features.ClearBasket;

public record ClearBasketCommand(Guid BasketId) : IRequest<Result>;

public class ClearBasketCommandHandler(
    IBasketRepository basketRepository,
    IBasketUnitOfWork unitOfWork
) : IRequestHandler<ClearBasketCommand, Result>
{
    public async Task<Result> Handle(ClearBasketCommand command, CancellationToken ct)
    {
        var basket = await basketRepository.GetByIdAsync(command.BasketId, ct);
        if (basket == null)
        {
            return Result.Failure(BasketErrors.NotFound);
        }

        basket.Clear();
        basketRepository.Update(basket);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
