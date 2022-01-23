using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Domain.Model;

namespace BuildingBlocks.Core.Domain.Events;

// https://www.ledjonbehluli.com/posts/domain_to_integration_event/
public interface IEventMapper<in T, TId>
    where T : IAggregateRoot<TId>
{
    IReadOnlyList<IIntegrationEvent> MapToIntegrationEvents(T aggregate);
}
