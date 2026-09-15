using Ordering.Domain.Common;

namespace Ordering.Domain.Orders.Events;

/// <summary>
/// Raised when a new order is created for a customer.
/// </summary>
/// <param name="OrderId">The identity of the created order.</param>
/// <param name="CustomerId">The identity of the customer who owns the order.</param>
/// <param name="OccurredAtUtc">The UTC timestamp when the event was recorded.</param>
public sealed record OrderCreatedDomainEvent(
    Guid OrderId,
    Guid CustomerId,
    DateTime OccurredAtUtc) : IDomainEvent;
