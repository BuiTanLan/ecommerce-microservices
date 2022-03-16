using Ardalis.GuardClauses;
using ECommerce.Services.Customers.RestockSubscriptions.Features.UpdatingMongoRestockSubscriptionReadModel;
using ECommerce.Services.Customers.RestockSubscriptions.Models.Write;
using MicroBootstrap.Abstractions.Core.Domain.Events.Internal;
using MicroBootstrap.Abstractions.CQRS.Command;
using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.ProcessingRestockNotification;

public record RestockNotificationProcessed(RestockSubscription RestockSubscription) : DomainEvent;

internal class RestockNotificationProcessedHandler : IDomainEventHandler<RestockNotificationProcessed>
{
    private readonly ICommandProcessor _commandProcessor;

    public RestockNotificationProcessedHandler(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public async Task Handle(RestockNotificationProcessed notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        // https://github.com/kgrzybek/modular-monolith-with-ddd#38-internal-processing
        await _commandProcessor.SendAsync(
            new UpdateMongoRestockSubscriptionReadModel(notification.RestockSubscription, false),
            cancellationToken);
    }
}
