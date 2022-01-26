using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;
using Catalog.Brands;
using Catalog.Categories;
using Catalog.Products.Core.Models.ValueObjects;
using Catalog.Products.Events.Domain;
using Catalog.Products.Exceptions.Domain;
using Catalog.Products.Features.ChangingProductBrand.Events.Domain;
using Catalog.Products.Features.ChangingProductCategory.Events;
using Catalog.Products.Features.ChangingProductPrice;
using Catalog.Products.Features.ChangingProductSupplier.Events;
using Catalog.Products.Features.CreatingProduct.Events.Domain;
using Catalog.Products.Features.DebitingProductStock.Events.Domain;
using Catalog.Products.Features.ReplenishingProductStock.Events.Domain;
using Catalog.Products.Models;
using Catalog.Suppliers;
using static BuildingBlocks.Core.Domain.Events.Internal.DomainEvents;

namespace Catalog.Products.Core.Models;

// https://event-driven.io/en/notes_about_csharp_records_and_nullable_reference_types/
// https://enterprisecraftsmanship.com/posts/link-to-an-aggregate-reference-or-id/
// https://ardalis.com/avoid-collections-as-properties/?utm_sq=grcpqjyka3
public class Product : AggregateRoot<ProductId>
{
    private readonly List<ProductImage> _images = new();

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public ProductStatus ProductStatus { get; private set; }
    public CategoryId CategoryId { get; private set; } = null!;
    public Category Category { get; private set; } = null!;
    public SupplierId SupplierId { get; private set; } = null!;
    public Supplier Supplier { get; private set; } = null!;
    public BrandId BrandId { get; private set; } = null!;
    public Brand Brand { get; private set; } = null!;
    public Dimensions Dimensions { get; private set; } = null!;
    public IReadOnlyList<ProductImage> Images => _images;

    /// <summary>
    /// Gets quantity in stock.
    /// </summary>
    public int AvailableStock { get; private set; }

    /// <summary>
    /// Gets available stock at which we should reorder.
    /// </summary>
    public int RestockThreshold { get; private set; }

    /// <summary>
    /// Gets maximum number of units that can be in-stock at any time (due to physicial/logistical constraints in warehouses).
    /// </summary>
    public int MaxStockThreshold { get; private set; }

    public static async Task<Product> CreateAsync(
        long id,
        string name,
        int stock,
        int restockThreshold,
        int maxStockThreshold,
        ProductStatus status,
        Dimensions dimensions,
        string? description,
        decimal price,
        Category? category,
        Supplier? supplier,
        Brand? brand,
        IList<ProductImage>? images = null)
    {
        await RaiseDomainEventAsync(
            new CreatingProduct(
                id,
                name,
                price,
                stock,
                restockThreshold,
                maxStockThreshold,
                status,
                dimensions.Width,
                dimensions.Height,
                dimensions.Depth,
                category,
                supplier,
                brand,
                description));

        var product = new Product
        {
            Id = id, RestockThreshold = Guard.Against.NegativeOrZero(restockThreshold, nameof(restockThreshold))
        };
        product.ChangeName(name);
        product.ChangeDescription(description);
        product.ChangePrice(price);
        product.ReplenishStock(stock);
        product.AddProductImages(images);
        product.ChangeStatus(status);
        product.ChangeDimensions(dimensions);
        product.ChangeMaxStockThreshold(maxStockThreshold);

        await product.ChangeCategory(category);
        await product.ChangeBrand(brand);
        await product.ChangeSupplier(supplier);

        product.AddDomainEvent(new ProductCreated(product));

        return product;
    }

    public void ChangeStatus(ProductStatus status)
    {
        ProductStatus = status;
    }

    public void ChangeDimensions(Dimensions dimensions)
    {
        Dimensions = Guard.Against.Null(dimensions, nameof(dimensions));
    }

    /// <summary>
    /// Sets catalog item name.
    /// </summary>
    /// <param name="name">The name to be changed.</param>
    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ProductDomainEventException("The catalog item name cannot be null, empty or whitespace.");

        Name = name;
    }

    /// <summary>
    /// Sets catalog item description.
    /// </summary>
    /// <param name="description">The description to be changed.</param>
    public void ChangeDescription(string? description)
    {
        Description = description;
    }

    /// <summary>
    /// Sets catalog item price.
    /// </summary>
    /// <remarks>
    /// Raise a <see cref="ProductPriceChanged"/>.
    /// </remarks>
    /// <param name="price">The price to be changed.</param>
    public void ChangePrice(decimal price)
    {
        if (price < 0)
            throw new ProductDomainEventException("The catalog item price cannot have negative value.");

        if (Price == price)
            return;

        Price = price;

        AddDomainEvent(new ProductPriceChanged(price));
    }

    /// <summary>
    /// Decrements the quantity of a particular item in inventory and ensures the restockThreshold hasn't
    /// been breached. If so, a RestockRequest is generated in CheckThreshold.
    /// </summary>
    /// <param name="quantity">The number of items to debit.</param>
    /// <returns>int: Returns the number actually removed from stock. </returns>
    public int DebitStock(int quantity)
    {
        if (HasStock(quantity) == false)
        {
            throw new InsufficientStockException($"Empty stock, product item {Name} is sold out");
        }

        if (quantity < 0) quantity *= -1;

        int removed = Math.Min(quantity, AvailableStock);

        AvailableStock -= removed;

        if (AvailableStock <= RestockThreshold)
        {
            AddDomainEvent(new ProductRestockThresholdReachedEvent(AvailableStock, quantity));
        }

        AddDomainEvent(new ProductStockDebited(AvailableStock, quantity));

        return removed;
    }

    /// <summary>
    /// Increments the quantity of a particular item in inventory.
    /// </summary>
    /// <returns>int: Returns the quantity that has been added to stock.</returns>
    /// <param name="quantity">The number of items to Replenish.</param>
    public int ReplenishStock(int quantity)
    {
        // we don't have enough space in the inventory
        if (AvailableStock + quantity > MaxStockThreshold)
        {
            throw new MaxStockThresholdReachedException(
                $"Max stock threshold has been reached. Max stock threshold is {MaxStockThreshold}");
        }

        AvailableStock += quantity;

        AddDomainEvent(new ProductStockReplenished(AvailableStock, quantity));

        return AvailableStock;
    }

    public void ChangeMaxStockThreshold(int maxStockThreshold)
    {
        Guard.Against.NegativeOrZero(maxStockThreshold, nameof(maxStockThreshold));

        MaxStockThreshold = maxStockThreshold;

        AddDomainEvent(new MaxThresholdChanged(maxStockThreshold));
    }

    public void ChangeRestockThreshold(int restockThreshold)
    {
        Guard.Against.NegativeOrZero(restockThreshold, nameof(restockThreshold));

        RestockThreshold = restockThreshold;

        AddDomainEvent(new RestockThresholdChanged(restockThreshold));
    }

    public bool HasStock(int quantity)
    {
        return AvailableStock >= quantity;
    }

    public void Activate() => ChangeStatus(ProductStatus.Available);

    public void DeActive() => ChangeStatus(ProductStatus.Unavailable);

    /// <summary>
    /// Sets category.
    /// </summary>
    /// <param name="category">The category to be changed.</param>
    public async Task ChangeCategory(Category? category)
    {
        Guard.Against.Null(category, nameof(category));
        Guard.Against.NullOrEmpty(category.Code, nameof(category.Code));
        Guard.Against.NullOrEmpty(category.Name, nameof(category.Name));
        Guard.Against.NegativeOrZero(category.Id, nameof(category.Id));

        // raising domain event immediately for checking some validation rule with some dependencies such as database
        await RaiseDomainEventAsync(new ChangingProductCategory(category.Id));

        Category = category;
        CategoryId = category.Id;

        // add event to domain events list that will be raise during commiting transaction
        AddDomainEvent(new ProductCategoryChanged(category.Id, Id));
    }

    /// <summary>
    /// Sets supplier.
    /// </summary>
    /// <param name="supplier">The supplier to be changed.</param>
    public async Task ChangeSupplier(Supplier? supplier)
    {
        Guard.Against.Null(supplier, nameof(supplier));
        Guard.Against.NullOrEmpty(supplier.Name, nameof(supplier.Name));
        Guard.Against.NegativeOrZero(supplier.Id, nameof(supplier.Id));

        await RaiseDomainEventAsync(new ChangingProductSupplier(supplier.Id));

        Supplier = supplier;
        SupplierId = supplier.Id;

        AddDomainEvent(new ProductSupplierChanged(supplier.Id, Id));
    }

    /// <summary>
    ///  Sets brand.
    /// </summary>
    /// <param name="brand">The brand to be changed.</param>
    public async Task ChangeBrand(Brand? brand)
    {
        Guard.Against.Null(brand, nameof(brand));
        Guard.Against.NullOrEmpty(brand.Name, nameof(brand.Name));
        Guard.Against.NegativeOrZero(brand.Id, nameof(brand.Id));

        await RaiseDomainEventAsync(new ChangingProductBrand(brand.Id));

        Brand = brand;
        BrandId = brand.Id;

        AddDomainEvent(new ProductBrandChanged(brand.Id, Id));
    }

    public void AddProductImages(IList<ProductImage>? productImages)
    {
        Guard.Against.Null(productImages, nameof(productImages));

        _images.AddRange(productImages);
    }
}
