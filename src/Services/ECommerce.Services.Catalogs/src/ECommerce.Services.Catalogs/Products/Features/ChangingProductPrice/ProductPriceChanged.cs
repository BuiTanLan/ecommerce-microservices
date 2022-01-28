using BuildingBlocks.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.ChangingProductPrice;

public record ProductPriceChanged(decimal Price) : DomainEvent;
