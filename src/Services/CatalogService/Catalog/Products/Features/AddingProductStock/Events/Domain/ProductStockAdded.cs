using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.AddingProductStock.Events.Domain;

public record ProductStockAdded(int NewStock) : DomainEvent;
