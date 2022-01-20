using BuildingBlocks.Domain.Exceptions;

namespace Catalog.Suppliers.Exceptions;

public class SupplierDomainException : DomainException
{
    public SupplierDomainException(string message) : base(message)
    {
    }

    public SupplierDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
