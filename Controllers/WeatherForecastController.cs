using Microsoft.AspNetCore.Mvc;

namespace azstream.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    //A GET endpoint that implements Server-Sent Events (SSE) to stream the list of summaries to the user, sleeping for one second between each stream event to simulate a slow data source.
    [HttpGet(Name = "GetWeatherForecastSSE")]
    public async Task GetSSE()
    {
        Response.Headers.Add("Content-Type", "text/event-stream");
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");

        foreach (var summary in Summaries)
        {
            await Response.WriteAsync($"data: {summary}\n\n");
            await Response.Body.FlushAsync();
            await Task.Delay(1000);
        }
    }

    //A GET endpoint that streams the list of summaries to the user, sleeping for one second between each stream event to simulate a slow data source.
    [HttpGet(Name = "GetWeatherForecastStream")]
    [Produces("text/event-stream")]
    public async IAsyncEnumerable<string> GetStream()
    {
        foreach (var summary in Summaries)
        {
            yield return summary;
            await Task.Delay(1000);
        }
    }
}
