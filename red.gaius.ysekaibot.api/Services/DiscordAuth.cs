using System.Data.Common;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;

namespace Red.Gaius.YsekaiBot.Services;

/// <summary>
/// Discord authentication service.
/// </summary>
public class DiscordAuth
{
    private readonly ILogger<DiscordAuth> logger;
    private readonly string discordId;
    private readonly string discordSecret;
    private readonly HttpClient httpClient;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordAuth"/> class.
    /// </summary>
    /// <param name="logger">Logging service.</param>
    /// <param name="config">Host configuration to retrieve database connection string.</param>
    /// <param name="httpClientFactory">Factory for http client instances.</param>
    /// <param name="cache">In-memory caching service.</param>
    public DiscordAuth(
        ILogger<DiscordAuth> logger,
        IConfiguration config,
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache)
    {
        this.logger = logger;

        var builder = new DbConnectionStringBuilder()
        {
            ConnectionString = config.GetConnectionString("DiscordClient"),
        };
        this.discordId = builder["client_id"].ToString() ?? string.Empty;
        this.discordSecret = builder["client_secret"].ToString() ?? string.Empty;

        this.httpClient = httpClientFactory.CreateClient("DiscordApi");
        this.cache = cache;
    }

    /// <summary>
    /// Get authentication code url to discord.
    /// </summary>
    /// <remarks>This endpoint authenticates users via Discord OAUTH2.</remarks>
    /// <param name="returnUrl">Url to return to after internal authentication.</param>
    /// <param name="redirectUrl">Url to return to after discord authentication.</param>
    /// <returns>Discord authentication code url.</returns>
    public string GetAuthCodeUrl(string returnUrl, string redirectUrl)
    {
        var authCodeUrl = QueryHelpers.AddQueryString(
            "https://discordapp.com/api/oauth2/authorize",
            new Dictionary<string, string?>()
            {
                { "response_type", "code" },
                { "client_id", this.discordId },
                { "scope", "identify email" },
                { "state", returnUrl },
                { "redirect_uri", redirectUrl },
            });
        this.logger.LogTrace("Generated Discord auth code url: { authCodeUrl }", authCodeUrl);
        return authCodeUrl;
    }

    /// <summary>
    /// Get access token from discord.
    /// </summary>
    /// <remarks>This endpoint authenticates users via Discord OAUTH2.</remarks>
    /// <param name="authCode">Authentication code.</param>
    /// <param name="redirectUrl">Url to return to after discord authentication.</param>
    /// <returns>Discord access token.</returns>
    public Dictionary<string, object> GetAccessToken(string authCode, string redirectUrl)
    {
        var query = new Dictionary<string, string?>()
        {
            { "client_id", this.discordId },
            { "client_secret", this.discordSecret },
            { "grant_type", "authorization_code" },
            { "code", authCode },
            { "redirect_uri", redirectUrl },
        };
        var response = this.httpClient.PostAsync("oauth2/token", new FormUrlEncodedContent(query)).Result;
        var accessToken = response.Content.ReadAsStringAsync().Result;
        this.logger.LogTrace("Recieved Discord access token: { accessToken }", accessToken);
        return JsonSerializer.Deserialize<Dictionary<string, object>>(
            accessToken,
            new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.WriteAsString,
            })!;
    }

    /// <summary>
    /// Get user data from discord.
    /// </summary>
    /// <remarks>This endpoint retrieves user data from Discord via access token.</remarks>
    /// <param name="accessToken">Discord access token.</param>
    /// <returns>Discord user data.</returns>
    public Dictionary<string, object> GetUserData(string accessToken)
    {
        if (this.cache.TryGetValue(accessToken, out Dictionary<string, object> cachedUserData))
        {
            return cachedUserData;
        }
        else
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "users/@me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = this.httpClient.Send(request).Content.ReadAsStringAsync().Result;
            this.logger.LogTrace("Recieved Discord user data: { userData }", response);
            var userData = JsonSerializer.Deserialize<Dictionary<string, object>>(
                response,
                new JsonSerializerOptions()
                {
                    NumberHandling = JsonNumberHandling.WriteAsString,
                })!;
            this.cache.Set(accessToken, userData, DateTimeOffset.Now.AddHours(4));
            return userData;
        }
    }
}