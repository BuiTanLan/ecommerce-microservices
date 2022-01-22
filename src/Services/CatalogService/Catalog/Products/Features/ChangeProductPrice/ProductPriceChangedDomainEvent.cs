using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Domain.Events;

namespace Catalog.Products.Features.ChangeProductPrice;

public record ProductPriceChangedDomainEvent(decimal Price) : DomainEvent;
