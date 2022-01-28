using BuildingBlocks.Web;

namespace ECommerce.Services.Catalogs.Products;

// https://github.com/domaindrivendev/Swashbuckle.AspNetCore#decorate-individual-actions
[ApiExplorerSettings(GroupName = "Product")]
[ApiVersion("1.0")]
// [Authorize]
[Route(CatalogConfiguration.CatalogModulePrefixUri + ProductsConfigs.ProductsPrefixUri)]
public abstract class ProductEndpointBase : BaseController
{
}
