using MediatR;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Repositories;
using SharedKernel.Domain;

namespace Ordering.Application.Features.GetPaymentMethods;

public record GetPaymentMethodsQuery(bool? OnlyActive = true) : IRequest<Result<List<PaymentMethodDto>>>;

public class GetPaymentMethodsQueryHandler(
    IPaymentMethodRepository paymentMethodRepository
) : IRequestHandler<GetPaymentMethodsQuery, Result<List<PaymentMethodDto>>>
{
    public async Task<Result<List<PaymentMethodDto>>> Handle(GetPaymentMethodsQuery request, CancellationToken ct)
    {
        var methods = await paymentMethodRepository.GetAllAsync(request.OnlyActive, ct);
        var dtos = methods.Select(m => new PaymentMethodDto(
            m.Code,
            m.Name,
            m.Description,
            m.IconUrl,
            m.IsEnabled
        )).ToList();

        return Result.Success(dtos);
    }
}
