using SharedKernel.Domain.Events;

namespace SharedKernel.Domain.Entities;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity(TId id)
    {
        Id = id;
    }

    protected BaseEntity() { }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

public abstract class BaseEntity : BaseEntity<Guid>
{
    protected BaseEntity(Guid id) : base(id) { }
    protected BaseEntity() : base() { }
}
