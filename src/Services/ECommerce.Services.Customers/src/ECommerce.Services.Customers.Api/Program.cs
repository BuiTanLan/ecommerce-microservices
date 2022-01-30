using System.Reflection;
using Ben.Diagnostics;
using BuildingBlocks.Core;
using BuildingBlocks.Jwt;
using BuildingBlocks.Logging;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions.ApplicationBuilderExtensions;
using BuildingBlocks.Web.Extensions.ServiceCollectionExtensions;
using Customers;
using Customers.Api.Extensions.ApplicationBuilderExtensions;
using Customers.Api.Extensions.ServiceCollectionExtensions;
using ECommerce.Services.Customers;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
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
        Customers.Api.Program.GetLogPath(builder.Environment, loggingOptions) ?? "../logs/customers-service.log",
        outputTemplate: loggingOptions?.LogTemplate ??
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true);
});

builder.AddCustomSwagger(builder.Configuration, typeof(CustomersRoot).Assembly);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddCustomJwtAuthentication(builder.Configuration);

builder.Services.AddCustomAuthorization();

builder.AddCustomersModuleServices();

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

await app.ConfigureCustomersModule(environment, app.Logger);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapCustomersModuleEndpoints();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

await app.RunAsync();

namespace Customers.Api
{
    public partial class Program
    {
        public static string? GetLogPath(IWebHostEnvironment env, LoggerOptions loggerOptions)
            => env.IsDevelopment() ? loggerOptions.DevelopmentLogPath : loggerOptions.ProductionLogPath;
    }
}
