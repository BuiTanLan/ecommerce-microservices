using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Exception;
using ECommerce.Services.Catalogs.Products.Exceptions.Domain;

namespace ECommerce.Services.Catalogs.Products.Models.ValueObjects;

public class Name : ValueObject
{
    public string Value { get; }

    public Name(string value)
    {
        Guard.Against.NullOrEmpty(value, new ProductDomainException("Name can't be null mor empty."));

        Value = value;
    }

    public static implicit operator Name(string value) => new(value);
    public static implicit operator string(Name value) => Guard.Against.Null(value.Value, new ProductDomainException("Name can't be null."));

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
