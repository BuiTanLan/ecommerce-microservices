using ECommerce.Services.Catalogs.Categories;
using ECommerce.Services.Catalogs.Products.ValueObjects;
using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.ChangingProductCategory.Events;

public record ProductCategoryChangedNotification(CategoryId CategoryId, ProductId ProductId) : DomainEvent;
