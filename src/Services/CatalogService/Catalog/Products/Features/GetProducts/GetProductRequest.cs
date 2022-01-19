using BuildingBlocks.CQRS;
using BuildingBlocks.Web;

namespace Catalog.Products.Features.GetProducts;

public class GetProductRequest : IPageRequest
{
    public List<string>? Includes { get; set; }
    public List<FilterModel>? Filters { get; set; }
    public List<string>? Sorts { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
