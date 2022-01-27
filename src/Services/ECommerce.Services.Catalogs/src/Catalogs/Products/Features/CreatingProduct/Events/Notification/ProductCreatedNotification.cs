using BuildingBlocks.Core.Domain.Events.Internal;
using Catalogs.Products.Features.CreatingProduct.Events.Domain;

namespace Catalogs.Products.Features.CreatingProduct.Events.Notification;

public record ProductCreatedNotification(ProductCreated DomainEvent) : DomainNotificationEventWrapper<ProductCreated>(DomainEvent);

