using BuildingBlocks.Abstractions.Domain.Exceptions;

namespace ECommerce.Services.Catalogs.Products.Exceptions.Domain;

public class ProductDomainException : DomainException
{
    public ProductDomainException(string message) : base(message)
    {
    }
}
