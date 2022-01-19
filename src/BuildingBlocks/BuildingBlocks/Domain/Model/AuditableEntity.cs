namespace BuildingBlocks.Domain.Model;

public abstract class AuditableEntity<TKey> : Entity<TKey>, IAuditableEntity<TKey>
{
    public DateTime? LastModified { get; protected set; }
    public int? LastModifiedBy { get; set; }
}

public class AuditableEntity : AuditableEntity<Guid>
{
}
