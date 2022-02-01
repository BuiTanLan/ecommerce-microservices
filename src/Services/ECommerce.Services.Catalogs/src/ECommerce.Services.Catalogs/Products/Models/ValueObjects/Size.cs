using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Exception;
using ECommerce.Services.Catalogs.Products.Exceptions.Domain;

namespace ECommerce.Services.Catalogs.Products.Models.ValueObjects;

public class Size : ValueObject
{
    public string Value { get; private set; }

    public Size(string value)
    {
        Guard.Against.NullOrWhiteSpace(value, new ProductDomainException("Size can not be empty or null."));

        Value = value;
    }

    public static implicit operator Size(string value) => new(value);

    public static implicit operator string(Size value) =>
        Guard.Against.Null(value.Value, new ProductDomainException("Size can't be null."));

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
