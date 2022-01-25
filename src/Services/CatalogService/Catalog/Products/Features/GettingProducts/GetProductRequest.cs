using BuildingBlocks.CQRS;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions;
using Catalog.Products.Core.Models;
using Catalog.Products.Models;

namespace Catalog.Products.Features.GettingProducts;

// https://blog.codingmilitia.com/2022/01/03/getting-complex-type-as-simple-type-query-string-aspnet-core-api-controller/
// https://benfoster.io/blog/minimal-apis-custom-model-binding-aspnet-6/
public record GetProductRequest : IPageRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public IList<string>? Includes { get; set; } =
        new List<string>(new[] { nameof(Product.Supplier), nameof(Product.Category) });

    public IList<string>? Sorts { get; set; }
    public IList<FilterModel>? Filters { get; set; }

    public static ValueTask<GetProductRequest?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        var request = new GetProductRequest();

        var page = httpContext.Request.Query.Get<int>("Page");
        var pageSize = httpContext.Request.Query.Get<int>("PageSize");
        var sorts = httpContext.Request.Query.GetCollection<List<string>>("Sorts");
        var filters = httpContext.Request.Query.GetCollection<List<FilterModel>>("Filters");
        var includes = httpContext.Request.Query.GetCollection<List<string>>("Includes");

        request.Page = page > 0 ? page : request.Page;
        request.PageSize = pageSize > 0 ? pageSize : request.PageSize;
        request.Sorts = sorts.Any() ? sorts : request.Sorts;
        request.Filters = filters.Any() ? filters : request.Filters;
        request.Includes = includes.Any() ? includes : request.Includes;

        return ValueTask.FromResult<GetProductRequest?>(request);
    }
}
