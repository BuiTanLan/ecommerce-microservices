using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    ///     Register Swagger endpoints.
    ///     Hint: Minimal Api not supported api versioning in .Net6
    /// </summary>
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app,
        IApiVersionDescriptionProvider provider = null)
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            options =>
            {
                options.DocExpansion(DocExpansion.None);
                if (provider is null)
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
                else
                    foreach (var description in provider.ApiVersionDescriptions)
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
            });

        return app;
    }
}
