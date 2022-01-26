using BuildingBlocks.Core.Domain.Exceptions;

namespace Catalog.Products.Exceptions.Domain;

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string message) : base(message)
    {
    }
}
