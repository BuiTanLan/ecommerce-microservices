using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Catalogs.Suppliers.Exceptions.Domain;

public class SupplierDomainException : DomainException
{
    public SupplierDomainException(string message) : base(message)
    {
    }

    public SupplierDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
