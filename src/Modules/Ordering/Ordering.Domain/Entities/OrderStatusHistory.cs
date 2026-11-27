using Ordering.Domain.Enums;
using SharedKernel.Domain.Entities;

namespace Ordering.Domain.Entities;

public class OrderStatusHistory : BaseEntity<long>
{
    public long OrderId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedTime { get; private set; }
    public bool IsOrderCreation { get; private set; }

    private OrderStatusHistory() { }
}
