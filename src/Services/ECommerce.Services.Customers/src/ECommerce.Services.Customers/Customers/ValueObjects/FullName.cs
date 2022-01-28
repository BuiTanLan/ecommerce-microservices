using BuildingBlocks.Core.Domain.Model;
using ECommerce.Services.Customers.Customers.Exceptions;

namespace ECommerce.Services.Customers.Customers.ValueObjects;

public class LastName : ValueObject
{
    public string Value { get; }

    public LastName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is > 150 or < 2)
        {
            throw new InvalidFullNameException(value ?? "null");
        }

        Value = value;
    }

    public static implicit operator LastName(string value) => new(value);

    public static implicit operator string(LastName value) => value.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
