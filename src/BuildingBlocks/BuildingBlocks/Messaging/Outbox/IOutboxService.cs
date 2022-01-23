using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;

namespace BuildingBlocks.Messaging.Outbox;

// http://www.kamilgrzybek.com/design/the-outbox-pattern/
// https://event-driven.io/en/outbox_inbox_patterns_and_delivery_guarantees_explained/
// https://debezium.io/blog/2019/02/19/reliable-microservices-data-exchange-with-the-outbox-pattern/
public interface IOutboxService
{
    Task<IEnumerable<OutboxMessage>> GetAllUnsentOutboxMessagesAsync(
        EventType eventType = EventType.IntegrationEvent | EventType.DomainNotificationEvent,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<OutboxMessage>> GetAllOutboxMessagesAsync(
        EventType eventType = EventType.IntegrationEvent | EventType.DomainNotificationEvent,
        CancellationToken cancellationToken = default);

    Task CleanProcessedAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(params IIntegrationEvent[] integrationEvents);
    Task SaveAsync(params IDomainNotificationEvent[] domainNotificationEvents);

    Task PublishUnsentOutboxMessagesAsync(CancellationToken cancellationToken = default);
}
