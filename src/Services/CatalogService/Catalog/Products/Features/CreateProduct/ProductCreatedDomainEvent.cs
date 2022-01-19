using BuildingBlocks.Domain.Events;

namespace Catalog.Products.Features.CreateProduct;

public record ProductCreatedDomainEvent(
    long Id,
    string Name,
    string Description,
    decimal Price,
    decimal Stock,
    long CategoryId,
    long SupplierId) : DomainEvent;
