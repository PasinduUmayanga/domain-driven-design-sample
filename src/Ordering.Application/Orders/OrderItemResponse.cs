using Ordering.Domain.Orders;

namespace Ordering.Application.Orders;

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice)
{
    internal static OrderItemResponse FromOrderItem(OrderItem item) =>
        new(
            item.Id,
            item.ProductId,
            item.ProductName,
            item.UnitPrice,
            item.Quantity,
            item.TotalPrice);
}
