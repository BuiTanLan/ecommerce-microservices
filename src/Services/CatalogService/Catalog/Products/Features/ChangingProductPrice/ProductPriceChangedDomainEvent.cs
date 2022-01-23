using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.ChangingProductPrice;

public record ProductPriceChangedDomainEvent(decimal Price) : DomainEvent;
