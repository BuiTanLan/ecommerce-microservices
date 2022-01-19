using System.Reflection;
using Ben.Diagnostics;
using BuildingBlocks.Jwt;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions.ApplicationBuilderExtensions;
using BuildingBlocks.Web.Extensions.ServiceCollection;
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

builder.AddCustomSerilog();

builder.AddCustomSwagger(builder.Configuration, typeof(CatalogRoot).Assembly);

builder.Services.AddHttpContextAccessor();

builder.Services.AddCustomHealthCheck(healthBuilder => { });

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddCustomJwtAuthentication(builder.Configuration);

builder.Services.AddCustomAuthorization();

builder.AddCatalogModule();

var app = builder.Build();

var environment = app.Environment;

if (environment.IsDevelopment() || environment.IsEnvironment("docker"))
{
    app.UseDeveloperExceptionPage();

    // Minimal Api not supported versioning in .net 6
    app.UseCustomSwagger();
}

app.UseHttpsRedirection();

app.UseBlockingDetection();

app.UseProblemDetails();

app.UseRouting();

app.UseAppCors();

app.UseSerilogRequestLogging();

app.UseCustomHealthCheck();

await app.ConfigureCatalogModule(environment, app.Logger);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapCatalogModuleEndpoints();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

await app.RunAsync();


public partial class Program
{
}
