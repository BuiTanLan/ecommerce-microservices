using BuildingBlocks.Domain.Model;

namespace Catalog.Categories;

public class Category : AggregateRoot<long>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }

    public static Category Create(string name, string description = "")
    {
        return new Category { Name = name, Description = description };
    }
}
