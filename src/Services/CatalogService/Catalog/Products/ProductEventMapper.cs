using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Products.Features.CreatingProduct;
using Catalog.Products.Models;

namespace Catalog.Products;

public class ProductEventMapper : IEventMapper<Product>
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            ProductCreatedDomainEvent e =>
                new ProductCreatedIntegrationEvent(
                    e.Product.Id,
                    e.Product.Name,
                    e.Product.CategoryId,
                    e.Product.Category.Name,
                    e.Product.AvailableStock),
            _ => null
        };
    }

    public IReadOnlyList<IIntegrationEvent?> MapToIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents)
    {
        return domainEvents.Select(MapToIntegrationEvent).ToList().AsReadOnly();
    }

    public IDomainNotificationEvent? MapToDomainNotificationEvent(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            ProductCreatedDomainEvent e =>
                new ProductCreatedDomainNotificationEvent(e),

            _ => null
        };
    }

    public IReadOnlyList<IDomainNotificationEvent?> MapToDomainNotificationEvents(
        IReadOnlyList<IDomainEvent> domainEvents)
    {
        return domainEvents.Select(MapToDomainNotificationEvent).ToList().AsReadOnly();
    }
}
