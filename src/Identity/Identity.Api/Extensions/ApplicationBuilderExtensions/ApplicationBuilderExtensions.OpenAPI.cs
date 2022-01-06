namespace Microsoft.AspNetCore.Builder;

public static partial class ApplicationBuilderExtensions
{
    /// <summary>
    /// Register Swagger endpoints.
    /// </summary>
    public static IApplicationBuilder UseSwaggerEndpoints(this IApplicationBuilder app, string routePrefix)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Server v1");
            c.RoutePrefix = routePrefix;
        });

        return app;
    }
}