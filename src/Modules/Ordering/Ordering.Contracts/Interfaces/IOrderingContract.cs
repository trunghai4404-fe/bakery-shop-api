using Ordering.Contracts.DTOs;
using SharedKernel.Domain;

namespace Ordering.Contracts.Interfaces;

public interface IOrderingContract
{
    Task<Result<CreateOrderResultDto>> CreateOrderFromBasketAsync(CreateOrderFromBasketRequest request, CancellationToken ct = default);
}
