namespace BuildingBlocks.Domain.Model;

public abstract class AuditableEntity<TKey> : Entity<TKey>, IAuditableEntity<TKey>
{
    public DateTime Created { get; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
    public int? CreatedBy { get; protected set; }
    public DateTime? LastModified { get; protected set; }
    public int? LastModifiedBy { get; protected set; }
}

public class AuditableEntity : AuditableEntity<Guid>
{
}
