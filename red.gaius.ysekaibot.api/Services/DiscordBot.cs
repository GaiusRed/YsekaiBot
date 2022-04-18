using System.Data.Common;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Red.Gaius.YsekaiBot.Services;

/// <summary>
/// Discord bot service.
/// </summary>
public class DiscordBot : BackgroundService
{
    private readonly ILogger<DiscordBot> logger;
    private readonly DiscordSocketClient discordClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordBot"/> class.
    /// </summary>
    /// <param name="services">Host's service provider.</param>
    /// <param name="logger">Logging service.</param>
    /// <param name="config">Host configuration to retrieve discord bot connection string.</param>
    public DiscordBot(
        IServiceProvider services,
        ILogger<DiscordBot> logger,
        IConfiguration config)
    {
        this.logger = logger;

        var builder = new DbConnectionStringBuilder()
        {
            ConnectionString = config.GetConnectionString("DiscordBot"),
        };
        var discordToken = builder["Token"].ToString() ?? string.Empty;
        var discordDefaultPrefix = builder["DefaultPrefix"].ToString() ?? string.Empty;

        this.discordClient = new DiscordSocketClient();
        this.discordClient.Log += (log) =>
        {
            if (log.Severity == LogSeverity.Critical)
            {
                this.logger.LogCritical(log.Exception, "{ message }", log.Message);
            }

            if (log.Severity == LogSeverity.Debug)
            {
                this.logger.LogDebug(log.Exception, "{ message }", log.Message);
            }

            if (log.Severity == LogSeverity.Error)
            {
                this.logger.LogError(log.Exception, "{ message }", log.Message);
            }

            if (log.Severity == LogSeverity.Info)
            {
                this.logger.LogInformation(log.Exception, "{ message }", log.Message);
            }

            if (log.Severity == LogSeverity.Verbose)
            {
                this.logger.LogTrace(log.Exception, "{ message }", log.Message);
            }

            if (log.Severity == LogSeverity.Warning)
            {
                this.logger.LogWarning(log.Exception, "{ message }", log.Message);
            }

            return Task.CompletedTask;
        };

        var commandSvc = new CommandService();
        commandSvc.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        this.discordClient.MessageReceived += async (arg) =>
        {
            if (arg is SocketUserMessage msg)
            {
                int pos = 0;
                if (msg.HasCharPrefix(discordDefaultPrefix[0], ref pos) ||
                    msg.HasMentionPrefix(this.discordClient.CurrentUser, ref pos))
                {
                    await commandSvc.ExecuteAsync(
                        new SocketCommandContext(this.discordClient, msg), pos, services);
                }
            }
        };

        this.discordClient.LoginAsync(TokenType.Bot, discordToken);
        this.discordClient.StartAsync();
    }

    /// <summary>
    /// Stop discord bot async.
    /// </summary>
    /// <param name="cancellationToken">Notify when task is cancelled.</param>
    /// <returns>Async task for the bot.</returns>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        this.discordClient.StopAsync();
        return base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Dispose object when done.
    /// </summary>
    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        base.Dispose();
    }

    /// <summary>
    /// Run discord bot async.
    /// </summary>
    /// <param name="stoppingToken">Notify when task is stopped.</param>
    /// <returns>Async task for the bot.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(100, stoppingToken);
        }
    }
}