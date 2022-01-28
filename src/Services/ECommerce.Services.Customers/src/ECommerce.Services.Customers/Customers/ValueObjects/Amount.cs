using BuildingBlocks.Core.Domain.Model;
using ECommerce.Services.Customers.Customers.Exceptions;

namespace ECommerce.Services.Customers.Customers.ValueObjects;

public class Amount : ValueObject
{
    public decimal Value { get; }

    public Amount(decimal value)
    {
        if (value is < 0 or > 1000000)
        {
            throw new InvalidAmountException(value);
        }

        Value = value;
    }

    public static Amount Zero => new(0);

    public static implicit operator Amount(decimal value) => new(value);

    public static implicit operator decimal(Amount value) => value.Value;

    public static bool operator ==(Amount? a, Amount? b)
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

    public static bool operator !=(Amount a, Amount b) => !(a == b);

    public static bool operator >(Amount a, Amount b) => a.Value > b.Value;

    public static bool operator <(Amount a, Amount b) => a.Value < b.Value;

    public static bool operator >=(Amount a, Amount b) => a.Value >= b.Value;

    public static bool operator <=(Amount a, Amount b) => a.Value <= b.Value;

    public static Amount operator +(Amount a, Amount b) => a.Value + b.Value;

    public static Amount operator -(Amount a, Amount b) => a.Value - b.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
