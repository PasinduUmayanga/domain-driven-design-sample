using Ordering.Domain.Common;

namespace Ordering.Domain.Orders.Events;

/// <summary>
/// Raised when a pending order is successfully confirmed.
/// </summary>
/// <param name="OrderId">The identity of the confirmed order.</param>
/// <param name="CustomerId">The identity of the customer who owns the order.</param>
/// <param name="TotalAmount">The total order amount at confirmation time.</param>
/// <param name="OccurredAtUtc">The UTC timestamp when the event was recorded.</param>
public sealed record OrderConfirmedDomainEvent(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    DateTime OccurredAtUtc) : IDomainEvent;
