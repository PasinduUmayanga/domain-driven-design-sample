using Ordering.Domain.Common;

namespace Ordering.Application.Abstractions.DomainEvents;

/// <summary>
/// Publishes domain events recorded by aggregate roots after persistence succeeds.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches the supplied domain events to the configured publishing mechanism.
    /// </summary>
    /// <param name="domainEvents">The domain events collected from an aggregate root.</param>
    /// <param name="cancellationToken">A token that can cancel the dispatch operation.</param>
    Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}
