namespace BuildingBlocks.Abstractions.Domain.Projections;

public interface IHaveProjection
{
    void When(object @event);
}
