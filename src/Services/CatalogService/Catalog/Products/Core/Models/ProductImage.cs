using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;

namespace Catalog.Products.Core.Models;

public class ProductImage : Entity<EntityId>
{
    public ProductImage(long id, string imageUrl, bool isMain, long productId)
    {
        SetImageUrl(imageUrl);
        SetIsMain(isMain);
        Id = Guard.Against.NegativeOrZero(id, nameof(id));
        ProductId = Guard.Against.NegativeOrZero(productId, nameof(productId));
        Guard.Against.NegativeOrZero(productId, nameof(productId));
    }

    // Just for EF
    private ProductImage(){}

    public string ImageUrl { get; private set; } = default!;
    public bool IsMain { get; private set; }
    public Product Product { get; private set; } = null!;
    public ProductId ProductId { get; private set; }

    public void SetIsMain(bool isMain) => IsMain = isMain;
    public void SetImageUrl(string url) => ImageUrl = url;
}
