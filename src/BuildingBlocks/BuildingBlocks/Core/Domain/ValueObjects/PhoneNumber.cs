using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Exceptions;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Exception;

namespace BuildingBlocks.Core.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        Value = Guard.Against.InvalidPhoneNumber(value, new DomainException($"Phone number {value} is invalid."));
    }

    public static implicit operator string?(PhoneNumber? phoneNumber) => phoneNumber?.Value;

    public static implicit operator PhoneNumber?(string? phoneNumber) => phoneNumber == null ? null : new(phoneNumber);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
