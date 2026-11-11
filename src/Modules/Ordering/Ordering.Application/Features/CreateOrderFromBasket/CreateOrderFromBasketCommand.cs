using MediatR;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Entities;
using Ordering.Domain.Errors;
using Ordering.Domain.Repositories;
using Ordering.Domain.ValueObjects;
using SharedKernel.Domain;

namespace Ordering.Application.Features.CreateOrderFromBasket;

public record CreateOrderFromBasketCommand(
    CreateOrderFromBasketRequest Request
) : IRequest<Result<CreateOrderResultDto>>;

public class CreateOrderFromBasketCommandHandler(
    IOrderRepository orderRepository,
    IPaymentMethodRepository paymentMethodRepository,
    IShippingMethodRepository shippingMethodRepository,
    IOrderingUnitOfWork unitOfWork
) : IRequestHandler<CreateOrderFromBasketCommand, Result<CreateOrderResultDto>>
{
    public async Task<Result<CreateOrderResultDto>> Handle(CreateOrderFromBasketCommand command, CancellationToken ct)
    {
        var req = command.Request;

        if (req.Items == null || req.Items.Count == 0)
        {
            return Result.Failure<CreateOrderResultDto>(OrderingErrors.EmptyOrder);
        }

        // 1. Kiểm tra Phương thức thanh toán từ Database
        var paymentMethod = await paymentMethodRepository.GetByCodeAsync(req.PaymentMethod, ct);
        if (paymentMethod == null || !paymentMethod.IsEnabled)
        {
            return Result.Failure<CreateOrderResultDto>(OrderingErrors.InvalidPaymentMethod(req.PaymentMethod));
        }

        // 2. Kiểm tra Phương thức vận chuyển từ Database nếu là Delivery
        decimal shippingFee = 0;
        string? shippingFeeNote = null;
        string? shippingMethodCode = null;
        string? shippingMethodName = null;

        var isDelivery = string.Equals(req.FulfillmentType, "Delivery", StringComparison.OrdinalIgnoreCase);
        if (isDelivery)
        {
            var shipCode = string.IsNullOrWhiteSpace(req.ShippingMethod) ? "INTERNAL" : req.ShippingMethod;
            var shippingMethod = await shippingMethodRepository.GetByCodeAsync(shipCode, ct);
            if (shippingMethod == null || !shippingMethod.IsEnabled)
            {
                return Result.Failure<CreateOrderResultDto>(OrderingErrors.InvalidShippingMethod(shipCode));
            }

            shippingMethodCode = shippingMethod.Code;
            shippingMethodName = shippingMethod.Name;
            shippingFee = shippingMethod.Fee;
            shippingFeeNote = shippingMethod.FeeDescription;
        }

        // 3. Tạo danh sách OrderItems
        var orderItems = req.Items.Select(i => new OrderItem(
            i.ProductId,
            i.ProductName,
            i.VariantId,
            i.VariantName,
            i.Quantity,
            i.Price,
            i.DiscountPrice,
            i.DiscountPercent,
            i.FinalPrice,
            i.ProductImageUrl
        )).ToList();

        // 4. Tạo Order aggregate root
        var customerInfo = new OrderCustomerInfo(
            req.CustomerInfo.Name,
            req.CustomerInfo.Phone,
            req.CustomerInfo.Email
        );

        OrderShippingAddress? shippingAddress = null;
        if (isDelivery && req.ShippingAddress != null)
        {
            shippingAddress = new OrderShippingAddress(
                req.ShippingAddress.Address,
                req.ShippingAddress.ProvinceId,
                req.ShippingAddress.WardId,
                req.ShippingAddress.Latitude,
                req.ShippingAddress.Longitude
            );
        }

        var order = Order.Create(
            req.UserId,
            req.SessionId,
            req.FulfillmentType,
            customerInfo,
            shippingAddress,
            shippingMethodCode,
            shippingMethodName,
            shippingFee,
            shippingFeeNote,
            paymentMethod.Code,
            paymentMethod.Name,
            orderItems,
            req.Note
        );

        await orderRepository.AddAsync(order, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new CreateOrderResultDto(
            order.Id,
            order.OrderCode,
            order.TotalAmount,
            order.Status.ToString(),
            order.PaymentStatus.ToString()
        ));
    }
}
