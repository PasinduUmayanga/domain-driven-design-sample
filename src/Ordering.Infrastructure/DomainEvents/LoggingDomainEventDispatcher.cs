using Microsoft.Extensions.Logging;
using Ordering.Application.Abstractions.DomainEvents;
using Ordering.Domain.Common;

namespace Ordering.Infrastructure.DomainEvents;

public sealed class LoggingDomainEventDispatcher(
    ILogger<LoggingDomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        foreach (var domainEvent in domainEvents)
        {
            logger.LogInformation(
                "Domain event dispatched: {DomainEventType} at {OccurredAtUtc}",
                domainEvent.GetType().Name,
                domainEvent.OccurredAtUtc);
        }

        return Task.CompletedTask;
    }
}
