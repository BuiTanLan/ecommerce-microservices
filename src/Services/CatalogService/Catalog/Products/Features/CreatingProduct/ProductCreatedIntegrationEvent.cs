using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.External;
using Catalog.Shared.Core.Contracts;

namespace Catalog.Products.Features.CreatingProduct;

public record ProductCreatedIntegrationEvent(long Id, string Name, long CategoryId, string CategoryName, int Stock) :
    IntegrationEvent;

public class ProductCreatedIntegrationEventConsumer :
    IIntegrationEventHandler<ProductCreatedIntegrationEvent>,
    IIntegrationEventHandler<IntegrationEventWrapper<ProductCreatedDomainEvent>>
{
    private readonly ICatalogDbContext _dbContext;

    public ProductCreatedIntegrationEventConsumer(ICatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Handle(ProductCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        return Task.CompletedTask;
    }

    public Task Handle(
        IntegrationEventWrapper<ProductCreatedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        return Task.CompletedTask;
    }
}
