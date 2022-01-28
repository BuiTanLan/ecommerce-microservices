using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Catalogs.Products.Features.CreatingProduct.Events.Domain;

namespace ECommerce.Services.Catalogs.Products.Features.CreatingProduct.Events.Notification;

public record ProductCreatedNotification(ProductCreated DomainEvent) : DomainNotificationEventWrapper<ProductCreated>(DomainEvent);

