using BuildingBlocks.Core.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Domain.Events.Internal;

/// <summary>
/// Execute event handlers immediately
/// Domain Events - Before Persistence
/// Ref https://ardalis.com/immediate-domain-event-salvation-with-mediatr/
/// https://www.weeklydevtips.com/episodes/022
/// </summary>
public static class DomainEvents
{
    private static readonly Func<IMediator> _mediatorFunc =
        ServiceActivator.GetScope().ServiceProvider.GetRequiredService<IMediator>;

    public static async Task RaiseDomainEventsAsync(params IDomainEvent[] domainEvents)
    {
        var mediator = _mediatorFunc.Invoke();
        await mediator.DispatchDomainEventsAsync(domainEvents);
    }
}
