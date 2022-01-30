using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;

namespace ECommerce.Services.Catalogs.Products.Models;

public class ProductId : AggregateId
{
    public ProductId(long value) : base(value)
    {
        Guard.Against.NegativeOrZero(value, nameof(value));
    }

    public static implicit operator long(ProductId id) => id.Value;

    public static implicit operator ProductId(long id) => new(id);
}
