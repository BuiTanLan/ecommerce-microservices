using BuildingBlocks.Domain.Events.External;

namespace BuildingBlocks.Diagnostics.OpenTelemetry.Messaging.Events;

public class AfterSendMessage
{
    public AfterSendMessage(IIntegrationEvent eventData)
        => EventData = eventData;
    public IIntegrationEvent EventData { get; }
}
