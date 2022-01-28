using BuildingBlocks.Core.Domain.Model;
using ECommerce.Services.Customers.Customers.Exceptions;

namespace ECommerce.Services.Customers.Customers.ValueObjects;

// BirthDate
public class BirthDate : ValueObject
{
    public DateTime Value { get; }

    public BirthDate(DateTime value)
    {
        if (value == default)
        {
            throw new InvalidBirthDateException(value);
        }

        DateTime minDateOfBirth = DateTime.Now.AddYears(-115);
        DateTime maxDateOfBirth = DateTime.Now.AddYears(-15);

        // Validate the minimum age.
        if (value < minDateOfBirth || value > maxDateOfBirth)
        {
            throw new InvalidBirthDateException("The minimum age has to be 15 years.");
        }

        Value = value;
    }

    public static implicit operator BirthDate(DateTime value) => new(value);

    public static implicit operator DateTime(BirthDate value) => value.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
