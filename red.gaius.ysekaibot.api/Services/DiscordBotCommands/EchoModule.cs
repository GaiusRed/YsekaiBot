using Discord.Commands;

namespace Red.Gaius.YsekaiBot.Services.Modules;

/// <summary>
/// Ping and echo module for Discord Bot.
/// </summary>
public class EchoModule : ModuleBase<SocketCommandContext>
{
    /// <summary>
    /// Echoes a message.
    /// </summary>
    /// <param name="echo">The text to echo.</param>
    /// <returns>Async task for the module.</returns>
    [Command("echo")]
    [Summary("Echoes a message.")]
    public async Task EchoAsync([Remainder][Summary("The text to echo")] string echo)
    {
        await this.ReplyAsync(echo);
    }

    /// <summary>
    /// Returns server latency.
    /// </summary>
    /// <returns>Async task for the module.</returns>
    [Command("ping")]
    [Summary("Returns server latency.")]
    public async Task PingAsync()
    {
        await this.ReplyAsync(string.Format("Pong! Latency: {0}ms", this.Context.Client.Latency.ToString()));
    }
}
