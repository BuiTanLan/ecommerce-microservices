namespace BuildingBlocks.Core.Domain.Exceptions;

/// <summary>
/// Exception type for domain exceptions.
/// </summary>
public class DomainException : System.Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }

    public DomainException()
    {
    }
}
