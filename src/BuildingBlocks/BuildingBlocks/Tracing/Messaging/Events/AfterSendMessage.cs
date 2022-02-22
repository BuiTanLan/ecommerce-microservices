using BuildingBlocks.Abstractions.Domain.Events.External;

namespace BuildingBlocks.Tracing.Messaging.Events;

public class AfterSendMessage
{
    public AfterSendMessage(IIntegrationEvent eventData)
        => EventData = eventData;
    public IIntegrationEvent EventData { get; }
}
