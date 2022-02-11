namespace BuildingBlocks.Messaging.Outbox;

[Flags]
public enum EventType
{
    IntegrationEvent = 1,
    DomainNotificationEvent = 2,
    InternalCommand = 4
}
