using BuildingBlocks.Domain.Events.External;

namespace BuildingBlocks.Domain.Events;

public interface IBusPublisher
{
    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
