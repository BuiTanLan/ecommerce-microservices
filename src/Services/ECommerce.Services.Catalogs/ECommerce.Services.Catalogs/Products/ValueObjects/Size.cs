using Ardalis.GuardClauses;
using ECommerce.Services.Catalogs.Products.Exceptions.Domain;
using MicroBootstrap.Core.Exception;

namespace ECommerce.Services.Catalogs.Products.ValueObjects;

public record Size
{
    public string Value { get; private set; }

    public Size? Null => null;

    public static Size Create(string value)
    {
        return new Size
        {
            Value = Guard.Against.NullOrWhiteSpace(
                value,
                new ProductDomainException("Size can not be empty or null."))
        };
    }

    public static implicit operator Size(string value) => Create(value);

    public static implicit operator string(Size value) =>
        Guard.Against.Null(value.Value, new ProductDomainException("Size can't be null."));
}
