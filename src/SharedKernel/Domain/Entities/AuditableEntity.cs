using SharedKernel.Domain.Interfaces;

namespace SharedKernel.Domain.Entities;

public abstract class AuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }

    protected AuditableEntity(TId id) : base(id) { }
    protected AuditableEntity() { }
}

public abstract class AuditableEntity : AuditableEntity<Guid>
{
    protected AuditableEntity(Guid id) : base(id) { }
    protected AuditableEntity() : base(Guid.NewGuid()) { }
}
