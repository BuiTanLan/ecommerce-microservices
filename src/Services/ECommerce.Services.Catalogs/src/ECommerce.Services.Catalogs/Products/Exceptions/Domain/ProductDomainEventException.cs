using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Catalogs.Products.Exceptions.Domain;

public class ProductDomainEventException : DomainException
{
    public ProductDomainEventException(string message) : base(message)
    {
    }
}
