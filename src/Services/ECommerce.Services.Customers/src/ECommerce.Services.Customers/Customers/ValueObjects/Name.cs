using BuildingBlocks.Core.Domain.Model;
using ECommerce.Services.Customers.Customers.Exceptions;

namespace ECommerce.Services.Customers.Customers.ValueObjects;

public class Name : ValueObject
{
    public string FirstName { get; } = null!;
    public string LastName { get; } = null!;
    public string FullName => FirstName + " " + LastName;

    public static readonly Name Empty = new();
    public static readonly Name? Null = null;

    private Name() { }

    public Name(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length is > 100 or < 3)
        {
            throw new InvalidNameException(firstName ?? "null");
        }

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length is > 100 or < 3)
        {
            throw new InvalidNameException(lastName ?? "null");
        }

        FirstName = firstName;
        LastName = lastName;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}
