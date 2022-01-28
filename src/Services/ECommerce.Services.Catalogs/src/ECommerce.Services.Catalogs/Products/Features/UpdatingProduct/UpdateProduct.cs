using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Catalogs.Brands;
using ECommerce.Services.Catalogs.Categories;
using ECommerce.Services.Catalogs.Products.Models;
using ECommerce.Services.Catalogs.Products.Models.ValueObjects;
using ECommerce.Services.Catalogs.Shared.Core.Contracts;
using ECommerce.Services.Catalogs.Shared.Infrastructure.Extensions;
using ECommerce.Services.Catalogs.Suppliers;

namespace ECommerce.Services.Catalogs.Products.Features.UpdatingProduct;

public record UpdateProduct(
    long Id,
    string Name,
    decimal Price,
    int RestockThreshold,
    int MaxStockThreshold,
    ProductStatus Status,
    int Width,
    int Height,
    int Depth,
    long CategoryId,
    long SupplierId,
    long BrandId,
    string? Description = null) : IUpdateCommand;

internal class UpdateProductCommandHandler : ICommandHandler<UpdateProduct>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public UpdateProductCommandHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task<Unit> Handle(UpdateProduct command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var product = await _catalogDbContext.FindProductAsync(command.Id, cancellationToken);

        Guard.Against.NullProduct(product, command.Id);

        var category = await _catalogDbContext.FindCategoryAsync(command.CategoryId, cancellationToken);
        Guard.Against.NullCategory(category, command.CategoryId);

        var brand = await _catalogDbContext.FindBrandAsync(command.BrandId, cancellationToken);
        Guard.Against.NullBrand(brand, command.BrandId);

        var supplier = await _catalogDbContext.FindSupplierAsync(command.SupplierId, cancellationToken);
        Guard.Against.NullSupplier(supplier, command.SupplierId);

        await product!.ChangeCategory(category);
        await product.ChangeBrand(brand);
        await product.ChangeSupplier(supplier);

        product.ChangeDescription(command.Description);
        product.ChangeName(command.Name);
        product.ChangePrice(command.Price);
        product.ChangeStatus(command.Status);
        product.ChangeDimensions(new Dimensions(command.Width, command.Height, command.Depth));
        product.ChangeMaxStockThreshold(command.MaxStockThreshold);
        product.ChangeRestockThreshold(command.RestockThreshold);

        await _catalogDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
