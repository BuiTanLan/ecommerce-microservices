using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Messaging.Outbox;
using ECommerce.Services.Catalogs.Products.Models;
using ECommerce.Services.Catalogs.Shared.Core.Contracts;

namespace ECommerce.Services.Catalogs.Products.Features.CreatingProduct.Events.Domain;

public record ProductCreated(Product Product) : DomainEvent, IHaveNotificationEvent, IHaveExternalEvent;

internal class ProductCreatedHandler : IDomainEventHandler<ProductCreated>
{
    private readonly ICatalogDbContext _dbContext;

    public ProductCreatedHandler(ICatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(ProductCreated notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        var existed = await _dbContext.Set<ProductView>()
            .FirstOrDefaultAsync(x => x.ProductId == notification.Product.Id, cancellationToken);

        if (existed is null)
        {
            var productView = new ProductView
            {
                ProductId = notification.Product.Id,
                ProductName = notification.Product.Name,
                CategoryId = notification.Product.Category.Id,
                CategoryName = notification.Product.Category.Name,
                SupplierId = notification.Product.Supplier.Id,
                SupplierName = notification.Product.Supplier.Name,
                BrandId = notification.Product.Brand.Id,
                BrandName = notification.Product.Brand.Name,
            };

            await _dbContext.Set<ProductView>().AddAsync(productView, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

// Mapping domain event to integration event in domain event handler is better from mapping in command handler (for preserving our domain rule invariants).
internal class ProductCreatedDomainEventToIntegrationMappingHandler : IDomainEventHandler<ProductCreated>
{
    private readonly IOutboxService _outboxService;

    public ProductCreatedDomainEventToIntegrationMappingHandler(IOutboxService outboxService)
    {
        _outboxService = outboxService;
    }

    public Task Handle(ProductCreated domainEvent, CancellationToken cancellationToken)
    {
        // 1. Mapping DomainEvent To IntegrationEvent
        // 2. Save Integration Event to Outbox
        return Task.CompletedTask;
    }
}
