using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Exceptions;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Exception;

namespace BuildingBlocks.Core.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    public Email(string value)
    {
        Value = Guard.Against.InvalidEmail(value, new DomainException($"Email {value} is invalid."));
    }

    public static implicit operator Email?(string? value) => value is null ? null : new(value);

    public static implicit operator string?(Email? value) => value?.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
