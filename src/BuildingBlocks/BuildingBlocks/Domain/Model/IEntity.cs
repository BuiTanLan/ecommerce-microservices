namespace BuildingBlocks.Domain.Model;

public interface IEntity : IEntity<Guid>
{
}

public interface IEntity<out TId> : IIdentity<TId>
{
}
