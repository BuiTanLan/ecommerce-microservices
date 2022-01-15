namespace BuildingBlocks.Domain.Model;

public interface IAuditableEntity : IAuditableEntity<Guid>
{
}

public interface IAuditableEntity<out TId> : IEntity<TId>
{
    DateTime Created { get; }
    int? CreatedBy { get; }
    DateTime? LastModified { get; }
    int? LastModifiedBy { get; }
}
