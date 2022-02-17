using System.Reflection;
using BuildingBlocks.Core;
using BuildingBlocks.Jwt;
using BuildingBlocks.Logging;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions.ApplicationBuilderExtensions;
using BuildingBlocks.Web.Extensions.ServiceCollectionExtensions;
using Catalogs.Api.Extensions.ApplicationBuilderExtensions;
using Catalogs.Api.Extensions.ServiceCollectionExtensions;
using ECommerce.Services.Catalogs;
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

builder.Services.AddApplicationOptions(builder.Configuration);
var loggingOptions = builder.Configuration.GetSection(nameof(LoggerOptions)).Get<LoggerOptions>();

builder.AddCompression();
builder.AddCustomProblemDetails();

builder.AddCustomSerilog(config =>
{
    config.WriteTo.File(
        Program.GetLogPath(builder.Environment, loggingOptions) ?? "../logs/customers-service.log",
        outputTemplate: loggingOptions?.LogTemplate ??
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true);
});

builder.AddCustomSwagger(builder.Configuration, typeof(CatalogRoot).Assembly);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddCustomJwtAuthentication(builder.Configuration);

builder.Services.AddCustomAuthorization();

builder.AddCatalogModuleServices();

var app = builder.Build();

var environment = app.Environment;

if (environment.IsDevelopment() || environment.IsEnvironment("docker"))
{
    app.UseDeveloperExceptionPage();

    // Minimal Api not supported versioning in .net 6
    app.UseCustomSwagger();
}


ServiceActivator.Configure(app.Services);

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
    public static string? GetLogPath(IWebHostEnvironment env, LoggerOptions loggerOptions)
        => env.IsDevelopment() ? loggerOptions.DevelopmentLogPath : loggerOptions.ProductionLogPath;
}
