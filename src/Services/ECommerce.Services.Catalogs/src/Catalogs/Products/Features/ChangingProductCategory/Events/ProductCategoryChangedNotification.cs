using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalogs.Products.Features.ChangingProductCategory.Events;

public record ProductCategoryChangedNotification(long CategoryId, long ProductId) : DomainEvent;
