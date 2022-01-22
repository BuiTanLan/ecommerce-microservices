using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.CreateProduct.Events.Domain;

public record ChangingProductSupplierDomainEvent(long SupplierId) : DomainEvent;
