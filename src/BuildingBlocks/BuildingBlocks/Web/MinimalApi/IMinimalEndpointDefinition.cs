using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Web.MinimalApi;

public interface IMinimalEndpointDefinition
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}
