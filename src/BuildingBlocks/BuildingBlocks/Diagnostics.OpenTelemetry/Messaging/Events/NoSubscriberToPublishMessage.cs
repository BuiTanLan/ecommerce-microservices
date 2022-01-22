using BuildingBlocks.Core.Domain.Events.External;

namespace BuildingBlocks.Diagnostics.OpenTelemetry.Messaging.Events;

public class NoSubscriberToPublishMessage
{
    public NoSubscriberToPublishMessage(IIntegrationEvent eventData)
        => EventData = eventData;
    public IIntegrationEvent EventData { get; }
}
