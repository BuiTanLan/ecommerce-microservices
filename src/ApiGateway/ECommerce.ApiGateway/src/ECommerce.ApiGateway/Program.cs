using System.IdentityModel.Tokens.Jwt;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// https://docs.duendesoftware.com/identityserver/v5/bff/apis/remote/
// https://microsoft.github.io/reverse-proxy/articles/index.html
// https://microsoft.github.io/reverse-proxy/articles/authn-authz.html
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("yarp"))
    // .AddTransforms<AccessTokenTransformProvider>()
    .AddTransforms(transforms =>
    {
        // https://microsoft.github.io/reverse-proxy/articles/transforms.html
        transforms.AddRequestTransform(transform =>
        {
            var requestId = Guid.NewGuid().ToString("N");
            var correlationId = Guid.NewGuid().ToString("N");

            transform.ProxyRequest.Headers.Add("X-Request-Id", requestId);
            transform.ProxyRequest.Headers.Add("X-Correlation-Id", correlationId);

            return ValueTask.CompletedTask;
        });
    });



var app = builder.Build();

app.MapGet("/", async (HttpContext context) =>
{
    await context.Response.WriteAsync($"ECommerce Gateway");
});

app.MapReverseProxy();

await app.RunAsync();
