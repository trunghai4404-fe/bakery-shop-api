namespace Ordering.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Preparing = 3,
    ReadyForPickup = 4,
    Shipping = 5,
    Completed = 6,
    Cancelled = 7
}
