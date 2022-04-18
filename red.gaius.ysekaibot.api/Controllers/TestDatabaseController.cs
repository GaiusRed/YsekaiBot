using Microsoft.AspNetCore.Mvc;
using Red.Gaius.YsekaiBot.Api.Models;
using Red.Gaius.YsekaiBot.Services;

namespace Red.Gaius.YsekaiBot.Api.Controllers;

/// <summary>
/// Example API controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestDatabaseController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    };

    private readonly Database db;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestDatabaseController"/> class.
    /// </summary>
    /// <param name="db">Persistence service.</param>
    public TestDatabaseController(Database db)
    {
        this.db = db;
    }

    /// <summary>
    /// Test data persistance write.
    /// </summary>
    /// <returns>The Weather Forecast.</returns>
    [HttpPost(Name = "PostPersistWeatherForecast")]
    public WeatherForecast Post()
    {
        var data = new WeatherForecast
        {
            Date = DateTime.Now,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
        };
        this.db.TestWrite(data);
        return data;
    }

    /// <summary>
    /// Test data persistance read.
    /// </summary>
    /// <returns>The Weather Forecast.</returns>
    [HttpGet(Name = "GetPersistWeatherForecast")]
    public WeatherForecast Get()
    {
        return this.db.TestRead();
    }
}
