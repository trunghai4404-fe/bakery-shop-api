using SharedKernel.Commons;
using SharedKernel.Domain.Errors;

namespace Ordering.Domain.Errors;

public static class OrderingErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        ErrorCode.OrderNotFound,
        $"Không tìm thấy đơn hàng với mã '{id}'.");

    public static Error NotFoundByCode(string code) => Error.NotFound(
        ErrorCode.OrderNotFound,
        $"Không tìm thấy đơn hàng với mã đơn '{code}'.");

    public static Error InvalidPaymentMethod(string method) => Error.Validation(
        ErrorCode.InvalidPaymentMethod,
        $"Phương thức thanh toán '{method}' không hợp lệ hoặc đang tạm ngưng hoạt động.");

    public static Error InvalidShippingMethod(string method) => Error.Validation(
        ErrorCode.InvalidShippingMethod,
        $"Phương thức vận chuyển '{method}' không hợp lệ hoặc đang tạm ngưng hoạt động.");

    public static Error PaymentMethodNotFound(string code) => Error.NotFound(
        ErrorCode.InvalidPaymentMethod,
        $"Không tìm thấy phương thức thanh toán '{code}'.");

    public static Error ShippingMethodNotFound(string code) => Error.NotFound(
        ErrorCode.InvalidShippingMethod,
        $"Không tìm thấy phương thức vận chuyển '{code}'.");

    public static readonly Error EmptyOrder = Error.Validation(
        ErrorCode.EmptyOrder,
        "Đơn hàng phải có ít nhất 1 sản phẩm.");
}
