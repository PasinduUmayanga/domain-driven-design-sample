namespace Ordering.Domain.Common;

/// <summary>
/// Describes something meaningful that has already happened in the domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the UTC timestamp when the domain event was recorded.
    /// </summary>
    DateTime OccurredAtUtc { get; }
}
