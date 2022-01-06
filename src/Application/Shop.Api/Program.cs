using System.Reflection;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;
using Shop.Api;
using Shop.Api.Endpoints;
using Shop.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseCustomSerilog();
builder.Host.UseDefaultServiceProvider((env, c) =>
{
    if (env.HostingEnvironment.IsDevelopment() || env.HostingEnvironment.IsStaging())
    {
        c.ValidateScopes = true;
    }
});


var appOptions = builder.Configuration.GetSection(nameof(AppOptions)).Get<AppOptions>();
builder.Services.AddOptions<AppOptions>().Bind(builder.Configuration.GetSection(nameof(AppOptions)))
    .ValidateDataAnnotations();

builder.Services.AddControllers(options =>
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())));

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("api", policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

builder.Services.AddCustomHealthCheck(healthBuilder => { });
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddCustomVersioning();

builder.AddCustomSwagger(builder.Configuration, typeof(Root).Assembly);
;

builder.AddAuthentication();
builder.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("docker"))
{
    app.UseDeveloperExceptionPage();
    // Minimal Api not supported versioning in .net 6
    app.UseCustomSwagger();
}

app.UseCors("api");

app.UseRouting();

app.UseCustomHealthCheck();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/home", (HttpResponse res) => Results.Text("Minimal API"))
    .WithMetadata(new EndpointNameMetadata("home"))
    .AllowAnonymous();

app.MapWeatherForecastEndpoints();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGet("/", context => context.Response.WriteAsync("Shop Apis!"));
});


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}