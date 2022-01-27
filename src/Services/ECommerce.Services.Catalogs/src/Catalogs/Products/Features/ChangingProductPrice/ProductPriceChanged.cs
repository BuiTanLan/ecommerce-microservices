using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalogs.Products.Features.ChangingProductPrice;

public record ProductPriceChanged(decimal Price) : DomainEvent;
