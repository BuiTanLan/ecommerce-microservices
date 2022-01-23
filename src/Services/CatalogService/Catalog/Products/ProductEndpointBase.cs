using BuildingBlocks.Web;
using Catalog;
using Catalog.Products;

namespace PartnersManagement.Orders;

// https://github.com/domaindrivendev/Swashbuckle.AspNetCore#decorate-individual-actions
[ApiExplorerSettings(GroupName = "Product")]
[ApiVersion("1.0")]
// [Authorize]
[Route(CatalogConfiguration.CatalogModulePrefixUri + ProductsConfigs.ProductsPrefixUri)]
public abstract class ProductEndpointBase : BaseController
{
}
