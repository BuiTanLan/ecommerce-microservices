using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Exception;
using ECommerce.Services.Catalogs.Products.Exceptions.Domain;

namespace ECommerce.Services.Catalogs.Products.ValueObjects;

public class Name : ValueObject
{
    public string Value { get; private set; }

    public Name? Null => null;

    public static Name Create(string value)
    {
        return new Name
        {
            Value = Guard.Against.NullOrEmpty(value, new ProductDomainException("Name can't be null mor empty."))
        };
    }

    public static implicit operator Name(string value) => Create(value);

    public static implicit operator string(Name value) =>
        Guard.Against.Null(value.Value, new ProductDomainException("Name can't be null."));

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
