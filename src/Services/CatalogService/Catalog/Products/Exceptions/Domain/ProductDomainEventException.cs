using BuildingBlocks.Domain.Exceptions;

namespace Catalog.Products.Exceptions.Domain;

public class ProductDomainEventException : DomainException
{
    public ProductDomainEventException(string message) : base(message)
    {
    }

    public ProductDomainEventException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
