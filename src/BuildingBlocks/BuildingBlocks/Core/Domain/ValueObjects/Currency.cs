using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Exceptions;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Exception;

namespace BuildingBlocks.Core.ValueObjects;

public class Currency : ValueObject
{
    public string Value { get; }

    public Currency(string value)
    {
        Value = Guard.Against.InvalidCurrency(value, new DomainException($"Currency {value} is invalid."));
    }

    public static implicit operator Currency?(string? value) => value == null ? null : new(value);

    public static implicit operator string?(Currency? value) => value?.Value;

    public static bool operator ==(Currency? a, Currency? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a is not null && b is not null)
        {
            return a.Value.Equals(b.Value);
        }

        return false;
    }

    public static bool operator !=(Currency? a, Currency? b) => !(a == b);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
