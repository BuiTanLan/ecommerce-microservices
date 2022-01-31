using BuildingBlocks.CQRS;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions;
using ECommerce.Services.Catalogs.Products.Models;

namespace ECommerce.Services.Catalogs.Products.Features.GettingProducts;

// https://blog.codingmilitia.com/2022/01/03/getting-complex-type-as-simple-type-query-string-aspnet-core-api-controller/
// https://benfoster.io/blog/minimal-apis-custom-model-binding-aspnet-6/
public record GetProductsRequest(int Page = 1, int PageSize = 20, IList<string>? Includes = null, IList<FilterModel>?
    Filters = null, IList<string>? Sorts = null) : PageRequest(Page, PageSize, Includes, Filters, Sorts)
{
    public static ValueTask<GetProductsRequest?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        var page = httpContext.Request.Query.Get<int>("Page", 1);
        var pageSize = httpContext.Request.Query.Get<int>("PageSize", 20);
        var sorts = httpContext.Request.Query.GetCollection<List<string>>("Sorts");
        var filters = httpContext.Request.Query.GetCollection<List<FilterModel>>("Filters");
        var includes = httpContext.Request.Query.GetCollection<List<string>>(
            "Includes",
            new List<string> { nameof(Product.Supplier), nameof(Product.Category) });

        var request = new GetProductsRequest(page, pageSize, includes, filters, sorts);

        return ValueTask.FromResult<GetProductsRequest?>(request);
    }
}
