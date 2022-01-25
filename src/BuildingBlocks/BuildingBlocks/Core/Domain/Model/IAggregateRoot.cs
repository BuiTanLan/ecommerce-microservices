using BuildingBlocks.Core.Objects.Versioning;

namespace BuildingBlocks.Core.Domain.Model;

public interface IAggregateRoot<TId> : IEntity<TId>, IHaveVersion, IHaveAggregate
{
}

public interface IAggregateRoot<TIdentity, TId> : IAggregateRoot<TIdentity>
    where TIdentity : Identity<TId>
{
}

public interface IAggregateRoot : IAggregateRoot<AggregateId, long>
{
}
