using BuildingBlocks.Core.Domain.Model;
using ECommerce.Services.Customers.Customers.Exceptions;

namespace ECommerce.Services.Customers.Customers.ValueObjects;

public class Address : ValueObject
{
    public string Value { get; }

    public Address(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is > 200 or < 3)
        {
            throw new InvalidAddressException(value ?? "null");
        }

        Value = value.Trim();
    }

    public static implicit operator Address(string? value) => new(value);

    public static implicit operator string(Address value) => value.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
