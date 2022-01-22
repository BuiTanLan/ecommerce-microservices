namespace BuildingBlocks.Core.Domain.Events;

/// <summary>
/// Internal Message Bus.
/// </summary>
public interface IEventProcessor
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    public Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
}
