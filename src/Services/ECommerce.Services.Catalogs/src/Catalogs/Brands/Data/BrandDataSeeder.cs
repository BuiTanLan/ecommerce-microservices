using Bogus;
using BuildingBlocks.IdsGenerator;
using Catalogs.Shared.Core.Contracts;

namespace Catalogs.Brands.Data;

public class BrandDataSeeder : IDataSeeder
{
    private readonly ICatalogDbContext _context;

    public BrandDataSeeder(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        if (await _context.Brands.AnyAsync())
            return;

        // https://github.com/bchavez/Bogus
        // https://www.youtube.com/watch?v=T9pwE1GAr_U
        var brandFaker = new Faker<Brand>().CustomInstantiator(faker =>
        {
            var brand = Brand.Create(SnowFlakIdGenerator.NewId(), faker.Company.CompanyName());
            return brand;
        });
        var brands = brandFaker.Generate(5);

        await _context.Brands.AddRangeAsync(brands);
        await _context.SaveChangesAsync();
    }
}
