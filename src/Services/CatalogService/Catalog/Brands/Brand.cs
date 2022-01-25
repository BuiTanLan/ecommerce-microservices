using BuildingBlocks.Core.Domain.Model;
using Catalog.Brands.Exceptions.Domain;

namespace Catalog.Brands;

public class Brand : AggregateRoot<BrandId>
{
    public string Name { get; private set; } = null!;

    public static Brand Create(long id, string name)
    {
        var brand = new Brand { Id = id };

        brand.ChangeName(name);

        return brand;
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BrandDomainException("Name can't be white space or null.");

        Name = name;
    }
}
