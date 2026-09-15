namespace Ordering.Domain.Common;

/// <summary>
/// Marks the entry point to an aggregate consistency boundary.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the domain events recorded by this aggregate during the current unit of work.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    /// <summary>
    /// Records a domain event after the aggregate has completed a valid business state change.
    /// </summary>
    /// <param name="domainEvent">The event that describes what happened in the domain.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes recorded domain events after they have been dispatched or intentionally ignored.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
