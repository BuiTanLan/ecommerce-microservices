namespace BuildingBlocks.Messaging.Outbox;

public enum EventType
{
    IntegrationEvent = 1,
    DomainNotificationEvent = 2,
    InternalCommand = 3
}
