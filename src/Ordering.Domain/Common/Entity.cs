namespace Ordering.Domain.Common;

/// <summary>
/// Base type for domain objects with identity.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; }
}
