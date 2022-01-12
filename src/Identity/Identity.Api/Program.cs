using System.Reflection;
using Ben.Diagnostics;
using BuildingBlocks.Jwt;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions.ApplicationBuilderExtensions;
using Hellang.Middleware.ProblemDetails;
using Identity;
using Identity.Api.Extensions.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;

//https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())));

builder.AddCompression();

builder.AddCustomProblemDetails();

builder.AddCustomSerilog();

builder.AddCustomSwagger(builder.Configuration, typeof(IdentityRoot).Assembly);

builder.Services.AddHttpContextAccessor();

builder.Services.AddCustomHealthCheck(healthBuilder => { });

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

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
