using Bogus;
using Catalog.Categories;
using Catalog.Core.Contracts;

namespace Catalog.Infrastructure.Data;

public class CatalogDataSeeder : IDataSeeder
{
    private readonly ICatalogDbContext _dbContext;

    public CatalogDataSeeder(ICatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAllAsync()
    {
        await SeedCategories();
        await SeedProducts();
    }

    private async Task SeedCategories()
    {
        if (await _dbContext.Categories.AnyAsync())
            return;

        // https://github.com/bchavez/Bogus
        // https://www.youtube.com/watch?v=T9pwE1GAr_U
        var categoryFaker = new Faker<Category>()
            .RuleFor(x => x.Name, f => f.Commerce.Categories(10).First())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence());

        var categories = categoryFaker.Generate(5);
        await _dbContext.Categories.AddRangeAsync(new List<Category>()
        {
            Category.Create("Electronics", "All electronic goods"),
            Category.Create("Clothing", "All clothing goods"),
            Category.Create("Books", "All books"),
        });
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedProducts()
    {
        if (await _dbContext.Products.AnyAsync())
            return;
    }
}
