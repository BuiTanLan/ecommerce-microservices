using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web;

public interface IModuleDefinition
{
    WebApplication ConfigureModule(WebApplication app);
    IServiceCollection AddModuleServices(IServiceCollection services);
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}
