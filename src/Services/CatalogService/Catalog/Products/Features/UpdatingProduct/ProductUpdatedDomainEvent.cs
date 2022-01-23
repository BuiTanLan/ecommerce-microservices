using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Products.Models;
using Catalog.Shared.Core.Contracts;

namespace Catalog.Products.Features.UpdatingProduct;

public record ProductUpdatedDomainEvent(Product Product) : DomainEvent;

public class ProductUpdatedDomainEventConsumer : IDomainEventHandler<ProductUpdatedDomainEvent>
{
    private readonly ICatalogDbContext _dbContext;

    public ProductUpdatedDomainEventConsumer(ICatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(ProductUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        var existed = await _dbContext.Set<ProductView>()
            .FirstOrDefaultAsync(x => x.ProductId == notification.Product.Id, cancellationToken);

        if (existed is not null)
        {
            existed.ProductId = notification.Product.Id;
            existed.ProductName = notification.Product.Name;
            existed.CategoryId = notification.Product.Category.Id;
            existed.CategoryName = notification.Product.Category.Name;
            existed.SupplierId = notification.Product.Supplier.Id;
            existed.SupplierName = notification.Product.Supplier.Name;
            existed.BrandId = notification.Product.Brand.Id;
            existed.BrandName = notification.Product.Brand.Name;

            _dbContext.Set<ProductView>().Update(existed);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
