using BuildingBlocks.Domain.Model;
using BuildingBlocks.IdsGenerator;
using Catalog.Suppliers.Exceptions;

namespace Catalog.Suppliers;

public class Supplier : AggregateRoot<long>
{
    public static Supplier Create(string name)
    {
        var supplier = new Supplier { Id = SnowFlakIdGenerator.NewId() };

        supplier.ChangeName(name);

        return supplier;
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new SupplierDomainException("Name can't be white space or null.");

        Name = name;
    }

    public string Name { get; private set; }
}
