using BuildingBlocks.Abstractions.Domain.Events.Internal;

namespace BuildingBlocks.Abstractions.Domain.Projections;

public interface IHaveProjection
{
    void When(IDomainEvent @event);
}
