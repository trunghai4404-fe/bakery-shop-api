using SharedKernel.Commons;
using SharedKernel.Domain.Errors;

namespace Ordering.Domain.Errors;

public static class OrderingErrors
{
    public static Error NotFound(long id) => Error.NotFound(
        ErrorCode.OrderNotFound,
        $"Không tìm thấy đơn hàng với ID '{id}'.");

    public static Error NotFoundByCode(string code) => Error.NotFound(
        ErrorCode.OrderNotFound,
        $"Không tìm thấy đơn hàng với mã đơn '{code}'.");

    public static Error NotFoundByCodeAndPhone => Error.NotFound(
        ErrorCode.OrderNotFound,
        "Không tìm thấy đơn hàng khớp với mã đơn và số điện thoại đã cung cấp.");

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

    public static readonly Error CannotCancelNonPendingOrder = Error.Validation(
        ErrorCode.ValidationError,
        "Chỉ có thể hủy đơn hàng khi đơn đang ở trạng thái Chờ xác nhận (Pending).");

    public static readonly Error OrderAlreadyCompleted = Error.Validation(
        ErrorCode.ValidationError,
        "Không thể thao tác hủy đơn hàng đã hoàn thành.");

    public static readonly Error OrderAlreadyCancelled = Error.Validation(
        ErrorCode.ValidationError,
        "Đơn hàng đã ở trạng thái hủy.");

    public static Error InvalidStatusTransition(string from, string to) => Error.Validation(
        ErrorCode.ValidationError,
        $"Không thể chuyển trạng thái đơn hàng từ '{from}' sang '{to}'.");

    public static Error InvalidStatus(string status) => Error.Validation(
        ErrorCode.ValidationError,
        $"Trạng thái đơn hàng '{status}' không hợp lệ.");

    public static Error InvalidPaymentStatus(string status) => Error.Validation(
        ErrorCode.ValidationError,
        $"Trạng thái thanh toán '{status}' không hợp lệ.");
}
