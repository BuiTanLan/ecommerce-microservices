using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;

namespace BuildingBlocks.Core.Domain.ValueObjects;

public class MoneyValue : ValueObject
{
    public static MoneyValue? Null => null;

    public decimal Value { get; }

    public string Currency { get; }

    public MoneyValue(decimal value, string currency)
    {
        Value = Guard.Against.NegativeOrZero(value, nameof(value));
        Currency = Guard.Against.NullOrWhiteSpace(currency, nameof(currency));
    }


    public static MoneyValue operator *(int left, MoneyValue right)
    {
        return new MoneyValue(right.Value * left, right.Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Currency;
    }
}
