namespace BuildingBlocks.Domain.Model;

public interface IAuditableEntity : IAuditableEntity<Guid>
{
}

public interface IAuditableEntity<out TId> : IEntity<TId>
{
    DateTime? LastModified { get; }
    int? LastModifiedBy { get; set; }
}
