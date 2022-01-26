using System.Reflection;
using Ben.Diagnostics;
using BuildingBlocks.Core;
using BuildingBlocks.Jwt;
using BuildingBlocks.Monitoring;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions.ApplicationBuilderExtensions;
using BuildingBlocks.Web.Extensions.ServiceCollectionExtensions;
using Catalog;
using Catalog.Api.Extensions.ApplicationBuilderExtensions;
using Catalog.Api.Extensions.ServiceCollectionExtensions;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;

// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis
// https://benfoster.io/blog/mvc-to-minimal-apis-aspnet-6/
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.AddCompression();
builder.AddCustomProblemDetails();

builder.AddCustomSerilog(config =>
{
    config.WriteTo.File(
        GetLogPath(builder.Environment),
        outputTemplate: CatalogConstants.LogTemplate,
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true);
});

builder.AddCustomSwagger(builder.Configuration, typeof(CatalogRoot).Assembly);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddCustomJwtAuthentication(builder.Configuration);

builder.Services.AddCustomAuthorization();

builder.AddCatalogServices();

var app = builder.Build();

var environment = app.Environment;

if (environment.IsDevelopment() || environment.IsEnvironment("docker"))
{
    app.UseDeveloperExceptionPage();

    // Minimal Api not supported versioning in .net 6
    app.UseCustomSwagger();
}


ServiceActivator.Configure(app.Services);

app.UseHttpsRedirection();

app.UseBlockingDetection();

app.UseProblemDetails();

app.UseRouting();

app.UseAppCors();

app.UseSerilogRequestLogging();

app.UseCustomHealthCheck();

await app.ConfigureCatalog(environment, app.Logger);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapCatalogEndpoints();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

await app.RunAsync();


public partial class Program
{
    private static string GetLogPath(IWebHostEnvironment env)
        => env.IsDevelopment() ? CatalogConstants.DevelopmentLogPath : CatalogConstants.ProductionLogPath;
}
