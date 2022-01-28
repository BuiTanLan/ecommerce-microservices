using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;

namespace ECommerce.Services.Customers.Customers.ValueObjects;

public class Nationality : ValueObject
{
    public string Value { get; }

    public Nationality(string? value)
    {
        Value = Guard.Against.InvalidNationality(value);
    }

    public static implicit operator Nationality(string value) => new(value);

    public static implicit operator string(Nationality value) => value.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
