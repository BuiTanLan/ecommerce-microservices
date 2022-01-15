using BuildingBlocks.EFCore;
using MediatR;

namespace BuildingBlocks.Domain.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;
    private readonly AppDbContextBase _context;
    private readonly IDomainEventContext _domainEventContext;

    public DomainEventDispatcher(IMediator mediator, AppDbContextBase context, IDomainEventContext domainEventContext)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _domainEventContext = domainEventContext;
    }

    public async Task DispatchEventsAsync()
    {
        var domainEvents = _domainEventContext.GetDomainEvents();

        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent);
    }
}
