using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Products.Models;
using Catalog.Shared.Core.Contracts;

namespace Catalog.Products.Features.CreatingProduct;

public record ProductCreatedDomainEvent(Product Product) : DomainEvent, IHaveNotificationEvent, IHaveExternalEvent;

public class ProductCreatedDomainEventConsumer : IDomainEventHandler<ProductCreatedDomainEvent>
{
    private readonly ICatalogDbContext _dbContext;

    public ProductCreatedDomainEventConsumer(ICatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        var existed = await _dbContext.Set<ProductView>()
            .FirstOrDefaultAsync(x => x.ProductId == notification.Product.Id, cancellationToken);

        if (existed is null)
        {
            var productView = new ProductView
            {
                ProductId = notification.Product.Id,
                ProductName = notification.Product.Name,
                CategoryId = notification.Product.Category.Id,
                CategoryName = notification.Product.Category.Name,
                SupplierId = notification.Product.Supplier.Id,
                SupplierName = notification.Product.Supplier.Name,
                BrandId = notification.Product.Brand.Id,
                BrandName = notification.Product.Brand.Name,
            };

            await _dbContext.Set<ProductView>().AddAsync(productView, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
