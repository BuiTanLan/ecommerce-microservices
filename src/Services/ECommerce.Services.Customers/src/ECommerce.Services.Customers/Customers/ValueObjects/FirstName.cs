using BuildingBlocks.Core.Domain.Model;
using ECommerce.Services.Customers.Customers.Exceptions;

namespace ECommerce.Services.Customers.Customers.ValueObjects;

public class FirstName : ValueObject
{
    public string Value { get; }

    public FirstName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is > 100 or < 3)
        {
            throw new InvalidNameException(value ?? "null");
        }

        Value = value.Trim().ToLowerInvariant().Replace(" ", ".", StringComparison.Ordinal);
    }

    public static implicit operator FirstName(string? value) => new(value);

    public static implicit operator string(FirstName value) => value.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
