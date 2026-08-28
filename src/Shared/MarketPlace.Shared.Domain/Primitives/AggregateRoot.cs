namespace MarketPlace.Shared.Domain.Primitives;

/// <summary>
/// Abstract base class for aggregate roots in domain-driven design.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    protected AggregateRoot()
    {
    }

    /// <summary>
    /// Initializes an aggregate root with a specific identifier.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    protected AggregateRoot(Guid id) : base(id)
    {
    }

    /// <summary>
    /// Gets the read-only collection of raised domain events.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Registers a domain event to be dispatched when changes are committed.
    /// </summary>
    /// <param name="domainEvent">The domain event instance.</param>
    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all raised domain events from the aggregate root.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
