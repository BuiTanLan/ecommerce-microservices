using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.ChangingProductSupplier.Events;

public record ProductSupplierChanged(long SupplierId, long ProductId) : DomainEvent;
