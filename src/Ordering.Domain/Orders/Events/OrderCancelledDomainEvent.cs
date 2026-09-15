using Ordering.Domain.Common;

namespace Ordering.Domain.Orders.Events;

/// <summary>
/// Raised when a pending order is successfully cancelled.
/// </summary>
/// <param name="OrderId">The identity of the cancelled order.</param>
/// <param name="CustomerId">The identity of the customer who owns the order.</param>
/// <param name="OccurredAtUtc">The UTC timestamp when the event was recorded.</param>
public sealed record OrderCancelledDomainEvent(
    Guid OrderId,
    Guid CustomerId,
    DateTime OccurredAtUtc) : IDomainEvent;
