using ECommerce.Services.Catalogs.Products.ValueObjects;
using ECommerce.Services.Catalogs.Suppliers;
using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.ChangingProductSupplier.Events;

public record ProductSupplierChanged(SupplierId SupplierId, ProductId ProductId) : DomainEvent;
