using BuildingBlocks.IdsGenerator;
using Catalog.Core.Contracts;

namespace Catalog.Categories.Data;

public class CategoryDataSeeder : IDataSeeder
{
    private readonly ICatalogDbContext _dbContext;

    public CategoryDataSeeder(ICatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAllAsync()
    {
        if (await _dbContext.Categories.AnyAsync())
            return;

        // https://github.com/bchavez/Bogus
        // https://www.youtube.com/watch?v=T9pwE1GAr_U
        // var categoryFaker = new Faker<Category>().CustomInstantiator(faker =>
        // {
        //     var category = Category.Create(faker.Commerce.Categories(10).First(),faker.Lorem.Sentence());
        //     return category;
        // });
        // var categories = categoryFaker.Generate(5);
        await _dbContext.Categories.AddRangeAsync(new List<Category>
        {
            Category.Create(SnowFlakIdGenerator.NewId(), "Electronics", "0001", "All electronic goods"),
            Category.Create(SnowFlakIdGenerator.NewId(), "Clothing", "0002", "All clothing goods"),
            Category.Create(SnowFlakIdGenerator.NewId(), "Books", "0003", "All books"),
        });
        await _dbContext.SaveChangesAsync();
    }
}
