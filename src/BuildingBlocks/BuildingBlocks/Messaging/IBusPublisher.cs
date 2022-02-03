using BuildingBlocks.Core.Domain.Events.External;

namespace BuildingBlocks.Messaging;

/// <summary>
/// External Message Bus.
/// </summary>
public interface IBusPublisher
{
    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
