using Ordering.Domain.Common;

namespace Ordering.Domain.Orders.Events;

/// <summary>
/// Raised when a product is added to an order or an existing product quantity is increased.
/// </summary>
/// <param name="OrderId">The identity of the order that received the item.</param>
/// <param name="ProductId">The identity of the product added to the order.</param>
/// <param name="ProductName">The product name captured at the time it was added.</param>
/// <param name="UnitPrice">The unit price captured at the time the product was added.</param>
/// <param name="Quantity">The quantity added by the operation.</param>
/// <param name="OccurredAtUtc">The UTC timestamp when the event was recorded.</param>
public sealed record OrderItemAddedDomainEvent(
    Guid OrderId,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    DateTime OccurredAtUtc) : IDomainEvent;
