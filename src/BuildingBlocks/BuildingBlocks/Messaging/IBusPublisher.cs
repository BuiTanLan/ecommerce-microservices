using BuildingBlocks.Domain.Events.External;

namespace BuildingBlocks.Domain.Events;

public interface IBusPublisher
{
    public Task PublishAsync<TEvent>(TEvent @event, string[] topics = default, CancellationToken token = default)
        where TEvent : IIntegrationEvent;
}
