using BuildingBlocks.Domain.Model;
using BuildingBlocks.IdsGenerator;
using Catalog.Categories;
using Catalog.Products.Exceptions.Domain;
using Catalog.Products.Features.AddProductStock;
using Catalog.Products.Features.ChangeProductPrice;
using Catalog.Products.Features.RemoveProductStock;
using Catalog.Suppliers;

namespace Catalog.Products;

// https://event-driven.io/en/notes_about_csharp_records_and_nullable_reference_types/
public class Product : AggregateRoot<long>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public long CategoryId { get; private set; }
    public virtual Category Category { get; private set; }
    public long SupplierId { get; private set; }
    public virtual Supplier Supplier { get; private set; }

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

    // Empty constructor for EF
    private Product() { }

    public static Product Create(
        string name,
        int stock,
        int restockThreshold,
        int maxStockThreshold,
        string description,
        decimal price,
        long categoryId,
        long supplierId)
    {
        var product = new Product
        {
            Id = SnowFlakIdGenerator.New(),
            MaxStockThreshold = maxStockThreshold,
            RestockThreshold = restockThreshold
        };

        product.ChangeName(name);
        product.ChangeCategoryId(categoryId);
        product.ChangeDescription(description);
        product.ChangePrice(price);
        product.ChangeSupplierId(supplierId);
        product.AddStock(stock);

        return product;
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
    public void ChangeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ProductDomainEventException("The product description cannot be null, empty or whitespace.");

        Description = description;
    }

    /// <summary>
    /// Sets catalog item price.
    /// </summary>
    /// <remarks>
    /// Raise a <see cref="ProductPriceChangedDomainEvent"/>.
    /// </remarks>
    /// <param name="price">The price to be changed.</param>
    public void ChangePrice(decimal price)
    {
        if (price < 0)
            throw new ProductDomainEventException("The catalog item price cannot have negative value.");

        if (Price == price)
            return;

        Price = price;

        AddDomainEvent(new ProductPriceChangedDomainEvent(price));
    }

    /// <summary>
    /// Decrements the quantity of a particular item in inventory and ensures the restockThreshold hasn't
    /// been breached. If so, a RestockRequest is generated in CheckThreshold.
    /// </summary>
    /// <returns>int: Returns the number actually removed from stock. </returns>
    public int RemoveStock(int quantity)
    {
        if (AvailableStock == 0)
        {
            throw new ProductDomainEventException($"Empty stock, product item {Name} is sold out");
        }

        if (quantity <= 0)
        {
            throw new ProductDomainEventException($"Item units desired should be greater than zero");
        }

        int removed = Math.Min(quantity, AvailableStock);

        AvailableStock -= removed;

        AddDomainEvent(new ProductStockRemovedDomainEvent(AvailableStock));

        return removed;
    }

    /// <summary>
    /// Increments the quantity of a particular item in inventory.
    /// <param name="quantity">The number of items to add in our stock.</param>
    /// <returns>int: Returns the quantity that has been added to stock</returns>
    /// </summary>
    public int AddStock(int quantity)
    {
        int original = AvailableStock;

        // The quantity that the client is trying to add to stock is greater than what can be physically accommodated in the Warehouse
        if (AvailableStock + quantity > MaxStockThreshold)
        {
            // For now, this method only adds new units up maximum stock threshold. In an expanded version of this application, we
            // could include tracking for the remaining units and store information about overstock elsewhere.
            AvailableStock += MaxStockThreshold - AvailableStock;
        }
        else
        {
            AvailableStock += quantity;
        }

        AvailableStock -= original;

        AddDomainEvent(new ProductStockAddedDomainEvent(AvailableStock));

        return AvailableStock;
    }

    /// <summary>
    /// Sets category identifier.
    /// </summary>
    /// <param name="categoryId">The categoryId to be changed.</param>
    public void ChangeCategoryId(long categoryId)
    {
        if (categoryId <= 0)
            throw new ProductDomainEventException("CategoryId should be greater than zero");
        CategoryId = categoryId;
    }

    /// <summary>
    /// Sets supplier identifier.
    /// </summary>
    /// <param name="supplierId">The supplierId to be changed.</param>
    public void ChangeSupplierId(long supplierId)
    {
        if (supplierId <= 0)
            throw new ProductDomainEventException("SupplierId should be greater than zero");
        SupplierId = supplierId;
    }
}
