using BuildingBlocks.Domain.Exceptions;

namespace Catalog.Brands.Exceptions;

public class BrandDomainException : DomainException
{
    public BrandDomainException(string message) : base(message)
    {
    }

    public BrandDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
