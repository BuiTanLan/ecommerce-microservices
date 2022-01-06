using Identity.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.AddSerilog();
builder.AddSwagger();
builder.Services.AddCors();
builder.AddCustomIdentityServer(builder.Configuration.GetConnectionString("ShopDBPostgressConnection"));

var app = builder.Build();

await EnsureDb(app.Services, app.Logger);

var environment = app.Environment;

if (environment.IsDevelopment() || environment.IsEnvironment("docker"))
{
    app.UseDeveloperExceptionPage()
        .UseSwaggerEndpoints(routePrefix: string.Empty);
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAppCors();

app.UseIdentityServer();

app.UseAuthorization();

app.MapGet("/", () => "Identity Server Apis").ExcludeFromDescription();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

var userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<ApplicationIdentityUser>>();
IdentityDataSeeder.SeedAll(userManager);

app.Run();

async Task EnsureDb(IServiceProvider services, ILogger logger)
{
    await using var db = services.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();
    if (db.Database.IsRelational())
    {
        logger.LogInformation("Updating database...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Updated database");
    }
}

public partial class Program { }