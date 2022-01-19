using BuildingBlocks.Domain.Events.External;
using BuildingBlocks.Domain.Model;

namespace BuildingBlocks.Domain.Events;

public interface IEventMapper<in T, TId>
    where T : IAggregateRoot<TId>
{
    IReadOnlyList<IIntegrationEvent> MapToIntegrationEvents(T aggregate);
    IReadOnlyList<IDomainNotificationEvent> MapToDomainEventNotification(T aggregate);
}
