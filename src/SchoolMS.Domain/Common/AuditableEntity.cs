namespace SchoolMS.Domain.Common;

public abstract class AuditableEntity : Entity
{
    protected AuditableEntity() { }
    protected AuditableEntity(Guid id) : base(id) { }
    public DateTimeOffset CreatedDateUtc { get; set; }
    public DateTimeOffset UpdatedDateUtc { get; set; }
}
