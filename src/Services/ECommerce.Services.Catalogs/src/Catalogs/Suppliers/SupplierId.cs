using BuildingBlocks.Core.Domain.Model;

namespace Catalogs.Suppliers;

public class SupplierId : AggregateId
{
    public SupplierId(long value) : base(value)
    {
    }

    public static implicit operator long(SupplierId id) => id.Value;

    public static implicit operator SupplierId(long id) => new(id);
}
