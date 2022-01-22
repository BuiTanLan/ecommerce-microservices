using BuildingBlocks.Core.Domain.Model;

namespace Catalog.Products.Models;

public class ProductImage : Entity<long>
{
    public ProductImage(long id, string imageUrl, bool isMain, long productId)
    {
        SetImageUrl(imageUrl);
        SetIsMain(isMain);
        ProductId = productId;
        Id = id;
    }

    // For EF
    public ProductImage() { }

    public string ImageUrl { get; private set; }
    public bool IsMain { get; private set; }
    public virtual Product Product { get; private set; }
    public long ProductId { get; private set; }

    public void SetIsMain(bool isMain) => IsMain = isMain;
    public void SetImageUrl(string url) => ImageUrl = url;
}
