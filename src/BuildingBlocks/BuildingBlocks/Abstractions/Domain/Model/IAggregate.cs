using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Domain.Projections;

namespace BuildingBlocks.Abstractions.Domain.Model;

public interface IAggregate<out TId> : IEntity<TId>, IHaveAggregate, IHaveEventSourcedAggregate, IHaveProjection
{
    void AddDomainEvent(IDomainEvent domainEvent);
    void CheckRule(IBusinessRule rule);
}

public interface IAggregate<out TIdentity, TId> : IAggregate<TIdentity>
    where TIdentity : Identity<TId>
{
}

public interface IAggregate : IAggregate<AggregateId, long>
{
}
