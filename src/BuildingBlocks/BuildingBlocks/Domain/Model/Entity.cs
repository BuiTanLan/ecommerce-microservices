namespace BuildingBlocks.Domain.Model;

public abstract class Entity : Entity<Guid>
{
}

public abstract class Entity<TId> : IEntity<TId>
{
    /// <inheritdoc />
    public TId Id { get; protected set; }
}
