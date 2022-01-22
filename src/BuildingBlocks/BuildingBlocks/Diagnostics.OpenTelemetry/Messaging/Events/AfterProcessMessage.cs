using BuildingBlocks.Core.Domain.Events.External;

namespace BuildingBlocks.Diagnostics.OpenTelemetry.Messaging.Events;

public class AfterProcessMessage
{
    public AfterProcessMessage(IIntegrationEvent eventData)
        => EventData = eventData;
    public IIntegrationEvent EventData { get; }
}
