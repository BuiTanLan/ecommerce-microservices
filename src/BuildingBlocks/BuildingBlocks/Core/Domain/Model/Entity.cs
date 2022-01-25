namespace BuildingBlocks.Core.Domain.Model;

public abstract class Entity<TId> : IEntity<TId>
{
    protected Entity(TId id) => Id = id;
    protected Entity() { }

    public TId Id { get; protected set; }

    public DateTime Created { get; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
    public int? CreatedBy { get; set; }
}

public abstract class Entity<TIdentity, TId> : Entity<TIdentity>
    where TIdentity : Identity<TId>
{
}

public abstract class Entity : Entity<EntityId, long>, IEntity
{
}
