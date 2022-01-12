using System;
using System.Threading.Tasks;
using BuildingBlocks.CQRS.Event.External;
using MediatR;

namespace BuildingBlocks.CQRS.Event;

public class EventProcessor : IEventProcessor
{
    private readonly IMediator _mediator;
    private readonly IExternalEventProducer _externalEventProducer;

    public EventProcessor(
        IMediator mediator,
        IExternalEventProducer externalEventProducer
    )
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _externalEventProducer = externalEventProducer ?? throw new ArgumentNullException(nameof(externalEventProducer));
    }

    public async Task PublishAsync(params IEvent[] events)
    {
        foreach (var @event in events)
        {
            await _mediator.Publish(@event);

            if (@event is IExternalEvent externalEvent)
                await _externalEventProducer.PublishAsync(externalEvent);
        }
    }
}
