using Ardalis.GuardClauses;
using ECommerce.Services.Catalogs.Products.Exceptions.Domain;
using ECommerce.Services.Catalogs.Suppliers;
using MicroBootstrap.Core.Exception;

namespace ECommerce.Services.Catalogs.Products.ValueObjects;

public record SupplierInformation
{
    public SupplierInformation Null => null;

    public Name Name { get; }
    public SupplierId Id { get; }

    public SupplierInformation(SupplierId id, Name name)
    {
        Id = Guard.Against.Null(id, new ProductDomainException("SupplierId can not be null."));
        Name = Guard.Against.Null(name, new ProductDomainException("Name cannot be null."));
    }
}
