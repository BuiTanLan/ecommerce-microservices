using Ardalis.GuardClauses;
using MicroBootstrap.Abstractions.Core.Domain.Events.External;
using MicroBootstrap.Core.Domain.Events.External;
using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.CreatingProduct.Events.Integration;

public record ProductCreated(long Id, string Name, long CategoryId, string CategoryName, int Stock) :
    IntegrationEvent;

public class ProductCreatedConsumer :
    IIntegrationEventHandler<ProductCreated>,
    IIntegrationEventHandler<IntegrationEventWrapper<Events.Domain.ProductCreated>>
{
    public Task Handle(ProductCreated notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        return Task.CompletedTask;
    }

    public Task Handle(IntegrationEventWrapper<Events.Domain.ProductCreated> notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        return Task.CompletedTask;
    }
}
