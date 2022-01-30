using BuildingBlocks.Core.Domain.Model;
using ECommerce.Services.Customers.Customers.Exceptions;

namespace ECommerce.Services.Customers.Customers.ValueObjects;

public class Nationality : ValueObject
{
    private static readonly HashSet<string> _allowedNationality = new()
    {
        "IR",
        "DE",
        "FR",
        "ES",
        "GB",
        "US"
    };

    public string Value { get; }

    public static Nationality? Null => null;

    public Nationality(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 2)
        {
            throw new InvalidNationalityException(value ?? "null");
        }

        value = value.ToUpperInvariant();
        if (!_allowedNationality.Contains(value))
        {
            throw new UnsupportedNationalityException(value);
        }

        Value = value;
    }

    public static implicit operator Nationality?(string? value) => value is null ? null : new(value);

    public static implicit operator string?(Nationality? value) => value?.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
