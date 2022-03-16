using ECommerce.Services.Catalogs.Products.Features.CreatingProduct.Events.Domain;
using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.CreatingProduct.Events.Notification;

public record ProductCreatedNotification(ProductCreated DomainEvent) : DomainNotificationEventWrapper<ProductCreated>(DomainEvent);

