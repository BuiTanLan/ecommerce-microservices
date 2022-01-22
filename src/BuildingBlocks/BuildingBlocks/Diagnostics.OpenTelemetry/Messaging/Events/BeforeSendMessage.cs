using BuildingBlocks.Core.Domain.Events.External;

namespace BuildingBlocks.Diagnostics.OpenTelemetry.Messaging.Events;

public class BeforeSendMessage
{
    public BeforeSendMessage(IIntegrationEvent eventData)
        => EventData = eventData;
    public IIntegrationEvent EventData { get; }
}
