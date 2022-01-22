using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.CreateProduct.Events.Domain;

public record ProductSupplierChangedDomainEvent(long SupplierId, long ProductId) : DomainEvent;
