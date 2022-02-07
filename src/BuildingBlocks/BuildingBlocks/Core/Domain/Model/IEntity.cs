namespace BuildingBlocks.Core.Domain.Model;

public interface IEntity<out TId> : IHaveEntity
{
    TId Id { get; }
}

public interface IEntity<out TIdentity, in TId> : IEntity<TIdentity>
    where TIdentity : IIdentity<TId>
{
}

public interface IEntity : IEntity<EntityId>
{
}
