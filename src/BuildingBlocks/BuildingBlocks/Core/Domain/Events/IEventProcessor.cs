namespace BuildingBlocks.Core.Domain.Events;

/// <summary>
/// Internal Message Bus.
/// </summary>
public interface IEventProcessor
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    public Task PublishAsync(IEvent[] events, CancellationToken cancellationToken = default);

    public Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    public Task DispatchAsync(IEvent[] events, CancellationToken cancellationToken = default);
}
