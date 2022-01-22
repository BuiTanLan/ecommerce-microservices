namespace BuildingBlocks.Core.Domain.Model;

/// <summary>
/// The aggregate root base class.
/// </summary>
/// <typeparam name="TId">The generic identifier.</typeparam>
public abstract class AuditAggregateRoot<TId> : AggregateRoot<TId>, IAuditableEntity<TId>
{
    public DateTime? LastModified { get; protected set; }
    public int? LastModifiedBy { get; set; }
}

public abstract class AuditAggregateRoot : AuditAggregateRoot<Guid>
{
}
