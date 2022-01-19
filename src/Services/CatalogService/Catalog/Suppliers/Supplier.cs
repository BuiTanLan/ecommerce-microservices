using BuildingBlocks.Domain.Model;

namespace Catalog.Suppliers;

public class Supplier : Entity<long>
{
    public Supplier(long id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Name { get; private set; }
}
