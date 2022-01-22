using BuildingBlocks.Core.Domain.Events.Internal;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace BuildingBlocks.Core.Extensions;

public static class MediatorExtensions
{
    public static Task DispatchDomainEventsAsync(this IMediator mediator, IEnumerable<IDomainEvent> domainEvents)
    {
        return PublishDomainEventsAsync(mediator, domainEvents);
    }

    private static async Task PublishDomainEventsAsync(IMediator mediator, IEnumerable<IDomainEvent> domainEvents)
    {
        var tasks = domainEvents
            .Select(async domainEvent =>
            {
                await mediator.Publish(domainEvent);
                Log.Logger.Debug(
                    "Published domain event {DomainEventName} with payload {DomainEventContent}",
                    domainEvent.GetType().FullName,
                    JsonConvert.SerializeObject(domainEvent));
            });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
