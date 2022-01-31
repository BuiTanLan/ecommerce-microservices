using BuildingBlocks.CQRS;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions;
using ECommerce.Services.Customers.Customers.Models;

namespace ECommerce.Services.Catalogs.Products.Features.GettingProducts;

// https://blog.codingmilitia.com/2022/01/03/getting-complex-type-as-simple-type-query-string-aspnet-core-api-controller/
// https://benfoster.io/blog/minimal-apis-custom-model-binding-aspnet-6/
public record GetCustomersRequest(int Page = 1, int PageSize = 20, CustomerState CustomerState = CustomerState.None,
        IList<string>? Includes = null, IList<FilterModel>? Filters = null, IList<string>? Sorts = null)
    : PageRequest(Page, PageSize, Includes, Filters, Sorts)
{
    public static ValueTask<GetCustomersRequest?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        var page = httpContext.Request.Query.Get<int>("Page", 1);
        var pageSize = httpContext.Request.Query.Get<int>("PageSize", 20);
        var customerState = httpContext.Request.Query.Get<CustomerState>("CustomerState", CustomerState.None);
        var sorts = httpContext.Request.Query.GetCollection<List<string>>("Sorts");
        var filters = httpContext.Request.Query.GetCollection<List<FilterModel>>("Filters");
        var includes = httpContext.Request.Query.GetCollection<List<string>>("Includes");

        var request = new GetCustomersRequest(page, pageSize, customerState, includes, filters, sorts);

        return ValueTask.FromResult<GetCustomersRequest?>(request);
    }
}
