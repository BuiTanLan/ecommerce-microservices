using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.CreatingProduct;

public record ProductCreatedDomainNotificationEvent(ProductCreatedDomainEvent DomainEvent) : DomainNotificationEventWrapper<ProductCreatedDomainEvent>(DomainEvent);

