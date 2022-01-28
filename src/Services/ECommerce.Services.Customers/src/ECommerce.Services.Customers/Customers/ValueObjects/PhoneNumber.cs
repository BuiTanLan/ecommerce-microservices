using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;

namespace ECommerce.Services.Customers.Customers.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        Value = Guard.Against.InvalidPhoneNumber(value);
    }

    public static implicit operator string(PhoneNumber phoneNumber)
    {
        return phoneNumber.Value;
    }

    public static implicit operator PhoneNumber(string phoneNumber)
    {
        return new PhoneNumber(phoneNumber);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
