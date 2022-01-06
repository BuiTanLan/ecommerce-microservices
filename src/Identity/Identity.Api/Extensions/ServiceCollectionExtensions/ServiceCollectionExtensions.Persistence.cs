namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddStorage(this WebApplicationBuilder builder)
    {
        // builder.Services.AddScoped(_ => new SqliteConnection("Data Source=todos.db"));

        return builder;
    }
}