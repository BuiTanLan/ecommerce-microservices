using BuildingBlocks.Domain.Events.External;

namespace BuildingBlocks.Diagnostics.OpenTelemetry.Messaging.Events;

public class BeforeProcessMessage
{
    public BeforeProcessMessage(IIntegrationEvent eventData)
        => EventData = eventData;
    public IIntegrationEvent EventData { get; }
}
