using BuildingBlocks.Core.Domain.Model;
using Catalog.Brands.Exceptions;
using Catalog.Brands.Exceptions.Domain;

namespace Catalog.Brands;

public class Brand : AggregateRoot<long>
{
    public static Brand Create(long id, string name)
    {
        var brand = new Brand { Id = id };

        brand.ChangeName(name);

        return brand;
    }

    // Empty constructor for EF - Here because we don't have other constructor we can remove it
    private Brand() { }

    public string Name { get; private set; }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BrandDomainException("Name can't be white space or null.");

        Name = name;
    }
}
