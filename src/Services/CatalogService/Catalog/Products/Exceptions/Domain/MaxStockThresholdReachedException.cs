using BuildingBlocks.Core.Domain.Exceptions;

namespace Catalog.Products.Exceptions.Domain;

public class MaxStockThresholdReachedException : DomainException
{
    public MaxStockThresholdReachedException(string message) : base(message)
    {
    }
}
