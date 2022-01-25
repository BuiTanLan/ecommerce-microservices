using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.ChangingProductPrice;

public record ProductPriceChanged(decimal Price) : DomainEvent;
