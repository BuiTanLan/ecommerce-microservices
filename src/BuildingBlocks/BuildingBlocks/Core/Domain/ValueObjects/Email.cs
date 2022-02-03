using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Exceptions;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Exception;

namespace BuildingBlocks.Core.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; }

    public static Email? Null => null;

    public static Email Create(string value)
    {
        return new Email
        {
            Value = Guard.Against.InvalidEmail(value, new DomainException($"Email {value} is invalid."))
        };
    }

    public static implicit operator Email?(string? value) => value is null ? null : Create(value);

    public static implicit operator string?(Email? value) => value?.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
