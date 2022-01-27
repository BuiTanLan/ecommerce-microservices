using BuildingBlocks.Core.Domain.Model;

namespace Catalogs.Products.Models;

public class ProductId : AggregateId
{
    public ProductId(long value) : base(value)
    {
    }

    public static implicit operator long(ProductId id) => id.Value;

    public static implicit operator ProductId(long id) => new(id);
}
