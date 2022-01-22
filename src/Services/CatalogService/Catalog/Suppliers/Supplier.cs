using BuildingBlocks.Core.Domain.Model;
using Catalog.Suppliers.Exceptions;
using Catalog.Suppliers.Exceptions.Domain;

namespace Catalog.Suppliers;

public class Supplier : AggregateRoot<long>
{
    public static Supplier Create(long id, string name)
    {
        var supplier = new Supplier { Id = id };

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
