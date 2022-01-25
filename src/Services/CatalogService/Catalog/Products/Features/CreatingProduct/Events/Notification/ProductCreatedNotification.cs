using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Products.Features.CreatingProduct.Events.Domain;

namespace Catalog.Products.Features.CreatingProduct;

public record ProductCreatedNotification(ProductCreated DomainEvent) : DomainNotificationEventWrapper<ProductCreated>(DomainEvent);

