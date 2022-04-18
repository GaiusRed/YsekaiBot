using LiteDB;
using Red.Gaius.YsekaiBot.Api.Models;

namespace Red.Gaius.YsekaiBot.Services;

/// <summary>
/// Persistence service using LiteDB.
/// </summary>
public class Database
{
    private readonly ILogger<Database> logger;
    private readonly LiteDatabase liteDb;

    /// <summary>
    /// Initializes a new instance of the <see cref="Database"/> class.
    /// </summary>
    /// <param name="logger">Logging service.</param>
    /// <param name="config">Host configuration to retrieve database connection string.</param>
    public Database(ILogger<Database> logger, IConfiguration config)
    {
        this.logger = logger;
        this.liteDb = new LiteDatabase(config.GetConnectionString("LiteDB"));

        this.logger.LogInformation("Database initialized.");
    }

    /// <summary>
    /// Updates or inserts user data to the database.
    /// </summary>
    /// <param name="user">User data to update or insert.</param>
    /// <returns>User data that was successfully updated or inserted.</returns>
    public User UpsertUser(User user)
    {
        var col = this.liteDb.GetCollection<User>();
        col.Upsert(user);
        col.EnsureIndex(i => i.Id);
        return this.GetUser(user.Id!);
    }

    /// <summary>
    /// Retrieves user data from the database.
    /// </summary>
    /// <param name="id">User data id.</param>
    /// <returns>User data that was successfully retrieved.</returns>
    public User GetUser(string id)
    {
        var col = this.liteDb.GetCollection<User>();
        return col.FindOne(i => i.Id!.Equals(id));
    }

    /// <summary>
    /// Tests data write.
    /// </summary>
    /// <param name="weatherForecast">Data to persist.</param>
    public void TestWrite(WeatherForecast weatherForecast)
    {
        this.logger.LogInformation("Testing data write.");
        try
        {
            var col = this.liteDb.GetCollection<WeatherForecast>("TestCollection");
            col.DeleteAll();
            col.Insert(weatherForecast);
            col.EnsureIndex(i => i.Date);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Test write failed.");
        }
    }

    /// <summary>
    /// Tests data read.
    /// </summary>
    /// <returns>Data that was persisted by TestWrite.</returns>
    public WeatherForecast TestRead()
    {
        this.logger.LogInformation("Testing data read.");
        try
        {
            var col = this.liteDb.GetCollection<WeatherForecast>("TestCollection");
            return col.FindOne(i => i.Summary!.Length > 0);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Test read failed.");
            throw;
        }
    }
}