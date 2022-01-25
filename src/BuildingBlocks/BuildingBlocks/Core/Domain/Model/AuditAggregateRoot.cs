namespace BuildingBlocks.Core.Domain.Model;

public abstract class AuditAggregateRoot<TId> : AggregateRoot<TId>, IAuditableEntity<TId>
{
    public DateTime? LastModified { get; protected set; }
    public int? LastModifiedBy { get; set; }
}

public abstract class AuditAggregateRoot<TIdentity, TId> : AuditAggregateRoot<TIdentity>
    where TIdentity : Identity<TId>
{
}

public abstract class AuditAggregateRoot : AuditAggregateRoot<Identity<long>, long>
{
}
