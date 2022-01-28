using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;

namespace ECommerce.Services.Customers.Customers.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    public Email(string value)
    {
        Value = Guard.Against.InvalidEmail(value);
    }


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
