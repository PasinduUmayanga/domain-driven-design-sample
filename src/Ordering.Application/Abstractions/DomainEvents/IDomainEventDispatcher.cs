using Ordering.Domain.Common;

namespace Ordering.Application.Abstractions.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}
