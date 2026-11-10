using SharedKernel.Commons;
using SharedKernel.Domain.Errors;

namespace Basket.Domain.Errors;

public static class BasketErrors
{
    public static readonly Error NotFound = 
        Error.NotFound(ErrorCode.BasketNotFound, "Giỏ hàng không tồn tại hoặc đã hết hạn.");

    public static readonly Error Empty = 
        Error.Validation(ErrorCode.BasketEmpty, "Giỏ hàng hiện đang rỗng.");

    public static readonly Error ShippingInfoRequired = 
        Error.Validation(ErrorCode.ShippingInfoRequired, "Hình thức giao hàng tận nơi bắt buộc phải có thông tin địa chỉ giao.");

    public static readonly Error InvalidQuantity = 
        Error.Validation(ErrorCode.InvalidQuantity, "Số lượng sản phẩm không hợp lệ.");

    public static Error OutOfStock(string productName, int availableStock) =>
        Error.Validation(ErrorCode.OutOfStock, $"Sản phẩm '{productName}' chỉ còn {availableStock} cái trong kho.");

    public static Error ProductNotFoundOrInactive(long variantId) =>
        Error.NotFound(ErrorCode.ProductNotFoundOrInactive, $"Biến thể sản phẩm (Mã {variantId}) không tồn tại hoặc đã ngừng kinh doanh.");
}