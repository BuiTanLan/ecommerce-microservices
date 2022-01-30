using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;
using ECommerce.Services.Catalogs.Suppliers.Exceptions.Domain;

namespace ECommerce.Services.Catalogs.Suppliers;

public class Supplier : AggregateRoot<SupplierId>
{
    public string Name { get; private set; } = default!;

    public static Supplier Create(SupplierId id, string name)
    {
        var supplier = new Supplier { Id = Guard.Against.Null(id, nameof(id)) };

        supplier.ChangeName(name);

        return supplier;
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new SupplierDomainException("Name can't be white space or null.");

        Name = name;
    }
}
