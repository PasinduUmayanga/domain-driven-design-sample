using Microsoft.Extensions.Logging;
using Ordering.Application.Abstractions.DomainEvents;
using Ordering.Domain.Common;

namespace Ordering.Infrastructure.DomainEvents;

/// <summary>
/// Demonstrates domain event dispatching by writing each event to application logs.
/// </summary>
public sealed class LoggingDomainEventDispatcher(
    ILogger<LoggingDomainEventDispatcher> logger) : IDomainEventDispatcher
{
    /// <inheritdoc />
    public Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        // A real system could publish these events to handlers, a message bus,
        // or an outbox table. Logging keeps this sample focused on the DDD flow.
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
