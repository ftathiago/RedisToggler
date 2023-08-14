using Microsoft.AspNetCore.Mvc;
using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Handlers;

namespace RedisToggler.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IDistributedTypedCache<CacheEntryConfiguration> _cache;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IDistributedTypedCache<CacheEntryConfiguration> cache)
    {
        _logger = logger;
        _cache = cache;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetAsync([FromQuery] bool fromCache)
    {
        const string key = "c3eed152-558b-42c1-93d8-9b87799b6aba";

        var obj = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        if (fromCache)
        {
            obj = await _cache.GetAsync(key, () => Task.FromResult(obj)!);
            await _cache.SetAsync(key, obj);
        }

        return obj!;
    }
}
