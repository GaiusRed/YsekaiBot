namespace Red.Gaius.YsekaiBot.Api.Models;

/// <summary>
/// Example Model.
/// </summary>
public class WeatherForecast
{
    /// <summary>
    /// Gets or sets the date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the temperature in Celsius.
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// Gets the temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);

    /// <summary>
    /// Gets or sets the summary.
    /// </summary>
    /// <example>["Freezing", "Bracing", "Chilly"].</example>
    public string? Summary { get; set; }
}
