using System.Security.Claims;

namespace Shop.Api.Endpoints;

public static class WeatherForecastEndpoints
{
    private const string Tag = "WeatherForecast";

    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public static WebApplication MapWeatherForecastEndpoints(this WebApplication app)
    {
        app.MapGet("api/v1/orders", GetWeathers)
            //.AllowAnonymous()
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<ICollection<WeatherForecast>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithDisplayName("Get all weathers");

        return app;
    }

    private static Task<IResult> GetWeathers(ClaimsPrincipal principal, ILogger<WeatherForecast> logger)
    {
        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToList();

        return Task.FromResult(Results.Ok(result));
    }
}
