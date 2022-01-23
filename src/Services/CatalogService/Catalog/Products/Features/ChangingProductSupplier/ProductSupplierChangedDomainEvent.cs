using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.ChangingProductSupplier;

public record ProductSupplierChangedDomainEvent(long SupplierId, long ProductId) : DomainEvent;
