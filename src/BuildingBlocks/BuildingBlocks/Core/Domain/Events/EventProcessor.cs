using Ardalis.GuardClauses;
using MediatR;

namespace BuildingBlocks.Core.Domain.Events;

public class EventProcessor : IEventProcessor
{
    private readonly IMediator _mediator;

    public EventProcessor(IMediator mediator)
    {
        _mediator = Guard.Against.Null(mediator, nameof(mediator));
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        return _mediator.Publish(@event, cancellationToken);
    }

    public async Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            await PublishAsync(@event, cancellationToken);
        }
    }
}
