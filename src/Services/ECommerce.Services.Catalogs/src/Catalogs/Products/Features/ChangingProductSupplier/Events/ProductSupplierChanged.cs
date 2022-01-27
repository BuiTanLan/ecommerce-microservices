using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalogs.Products.Features.ChangingProductSupplier.Events;

public record ProductSupplierChanged(long SupplierId, long ProductId) : DomainEvent;
