using System.Reflection;
using Ben.Diagnostics;
using BuildingBlocks.Jwt;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions.ApplicationBuilderExtensions;
using BuildingBlocks.Web.Extensions.ServiceCollectionExtensions;
using Hellang.Middleware.ProblemDetails;
using Identity;
using Identity.Api.Extensions.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

builder.AddCustomSwagger(builder.Configuration, typeof(IdentityRoot).Assembly);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddCustomJwtAuthentication(builder.Configuration);

builder.Services.AddCustomAuthorization();

builder.AddIdentityModule();

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

app.UseCustomHealthCheck();

await app.ConfigureIdentityModule(environment, app.Logger);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapIdentityModuleEndpoints();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

await app.RunAsync();


public partial class Program
{
}
