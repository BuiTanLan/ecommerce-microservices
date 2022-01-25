using BuildingBlocks.Core.Domain.Model;

namespace Catalog.Products.Models;

public class CategoryId : AggregateId
{
    public CategoryId(long value) : base(value)
    {
    }

    public static implicit operator long(CategoryId id) => id.Value;

    public static implicit operator CategoryId(long id) => new(id);
}
