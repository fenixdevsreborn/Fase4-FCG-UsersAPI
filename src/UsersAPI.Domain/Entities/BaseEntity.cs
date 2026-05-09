namespace UsersAPI.Domain.Entities;

/// <summary>
/// Base class for all domain entities.
/// Provides identity and audit information.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    private readonly List<object> _domainEvents = new();

    public IReadOnlyCollection<object> DomainEvents => _domainEvents;

    protected void AddDomainEvent(object @event)
        => _domainEvents.Add(@event);

    public void ClearDomainEvents()
        => _domainEvents.Clear();

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    protected void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
