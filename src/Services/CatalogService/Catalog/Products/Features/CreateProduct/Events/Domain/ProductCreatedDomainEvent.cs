using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Products.Models;

namespace Catalog.Products.Features.CreateProduct.Events.Domain;

public record ProductCreatedDomainEvent(Product Product) : DomainEvent;
