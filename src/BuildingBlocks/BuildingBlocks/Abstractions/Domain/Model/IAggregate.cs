using BuildingBlocks.Abstractions.Domain.Projections;
using BuildingBlocks.Core.Objects.Versioning;

namespace BuildingBlocks.Abstractions.Domain.Model;

public interface IAggregate<out TId> : IEntity<TId>, IHaveAggregate, IHaveVersion, IHaveProjection
{
    void IncrementVersion();
    void CheckRule(IBusinessRule rule);
}

public interface IAggregate<out TIdentity, TId> : IAggregate<TIdentity>
    where TIdentity : Identity<TId>
{
}

public interface IAggregate : IAggregate<AggregateId, long>
{
}
