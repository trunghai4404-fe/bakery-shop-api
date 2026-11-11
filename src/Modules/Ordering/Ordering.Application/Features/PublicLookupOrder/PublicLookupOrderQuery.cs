using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Errors;
using Ordering.Domain.Repositories;
using SharedKernel.Domain;

namespace Ordering.Application.Features.PublicLookupOrder;

public record PublicLookupOrderQuery(
    string OrderCode,
    string PhoneNumber
) : IRequest<Result<PublicOrderLookupDto>>;

public class PublicLookupOrderQueryHandler(
    IOrderRepository orderRepository
) : IRequestHandler<PublicLookupOrderQuery, Result<PublicOrderLookupDto>>
{
    public async Task<Result<PublicOrderLookupDto>> Handle(PublicLookupOrderQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.OrderCode) || string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            return Result.Failure<PublicOrderLookupDto>(OrderingErrors.NotFoundByCodeAndPhone);
        }

        var order = await orderRepository.GetByCodeAndPhoneAsync(
            request.OrderCode.Trim().ToUpper(),
            request.PhoneNumber.Trim(),
            ct);

        if (order == null)
        {
            return Result.Failure<PublicOrderLookupDto>(OrderingErrors.NotFoundByCodeAndPhone);
        }

        return Result.Success(order.ToPublicLookupDto());
    }
}
