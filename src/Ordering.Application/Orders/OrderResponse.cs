using Ordering.Domain.Orders;

namespace Ordering.Application.Orders;

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    DateTime CreatedAtUtc,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemResponse> Items)
{
    internal static OrderResponse FromOrder(Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.Status,
            order.CreatedAtUtc,
            order.TotalAmount,
            order.Items.Select(OrderItemResponse.FromOrderItem).ToArray());
}
