using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;

namespace BuildingBlocks.Core.Domain.ValueObjects;

public class Address : ValueObject
{
    public string Country { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string Detail { get; private set; } = null!;

    public static Address Empty => new();
    public static Address? Null => null;

    private Address() { }

    public static Address? Create(string? country, string? city, string? detail)
    {
        if (string.IsNullOrWhiteSpace(country) && string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(detail))
            return Null;

        Guard.Against.NullOrEmpty(country, nameof(country));
        Guard.Against.NullOrEmpty(city, nameof(city));
        Guard.Against.NullOrEmpty(detail, nameof(detail));

        var address = new Address { Country = country, City = city, Detail = detail };

        return address;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Country;
        yield return City;
        yield return Detail;
    }
}
