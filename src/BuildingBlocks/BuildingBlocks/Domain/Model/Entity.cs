namespace BuildingBlocks.Domain.Model;

public abstract class Entity<TId> : IEntity<TId>
{
    /// <inheritdoc />
    public TId Id { get; protected init; }
    public DateTime Created { get; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
    public int? CreatedBy { get; set; }
}

public abstract class Entity : Entity<Guid>
{
}
