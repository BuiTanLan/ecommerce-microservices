using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Exception;
using ECommerce.Services.Catalogs.Products.Exceptions.Domain;

namespace ECommerce.Services.Catalogs.Products.Models.ValueObjects;

public class Stock : ValueObject
{
    /// <summary>
    /// Gets quantity in stock.
    /// </summary>
    public int Available { get; private set; }

    /// <summary>
    /// Gets available stock at which we should reorder.
    /// </summary>
    public int RestockThreshold { get; private set; }

    /// <summary>
    /// Gets maximum number of units that can be in-stock at any time (due to physicial/logistical constraints in warehouses).
    /// </summary>
    public int MaxStockThreshold { get; private set; }

    public Stock(int available, int restockThreshold, int maxStockThreshold)
    {
        Available = Guard.Against.NegativeOrZero(
            available,
            new ProductDomainException("Available stock cannot be negative or zero."));
        RestockThreshold = Guard.Against.NegativeOrZero(
            restockThreshold,
            new ProductDomainException("Restock threshold cannot be negative or zero."));
        MaxStockThreshold = Guard.Against.NegativeOrZero(
            maxStockThreshold,
            new ProductDomainException("Max stock threshold cannot be negative or zero."));

        if (available > maxStockThreshold)
            throw new MaxStockThresholdReachedException("Available stock cannot be greater than max stock threshold.");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Available;
        yield return RestockThreshold;
        yield return MaxStockThreshold;
    }
}
