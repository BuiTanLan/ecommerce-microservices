using ECommerce.Services.Catalogs.Products.ValueObjects;
using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.ChangingProductPrice;

public record ProductPriceChanged(Price Price) : DomainEvent;
