using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Red.Gaius.YsekaiBot.Services;

namespace Red.Gaius.YsekaiBot.Api.Controllers;

/// <summary>
/// User controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> logger;
    private readonly Database db;
    private readonly DiscordAuth auth;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="logger">Logging service.</param>
    /// <param name="db">Persistence service.</param>
    /// <param name="auth">Discord authentication service.</param>
    public UserController(ILogger<UserController> logger, Database db, DiscordAuth auth)
    {
        this.logger = logger;
        this.db = db;
        this.auth = auth;
    }

    /// <summary>
    /// Retrieves user data via discord access token.
    /// </summary>
    /// <returns>User data.</returns>
    /// <response code="200">User data retrieved.</response>
    /// <response code="401">Access token not found or expired.</response>
    [HttpGet]
    public ActionResult Get()
    {
        var accessToken = string.Empty;
        if (this.Request.Cookies.Keys.Contains("__Secure-Access-Token"))
        {
            accessToken = this.Request.Cookies["__Secure-Access-Token"]!.Trim();
        }

        if (string.IsNullOrEmpty(accessToken))
        {
            return this.Unauthorized();
        }

        try
        {
            var discordUserData = this.auth.GetUserData(accessToken);
            return new JsonResult(this.db.UpsertUser(new Models.User(discordUserData)));
        }
        catch (Exception ex)
        {
            this.logger.LogError(exception: ex, "{ message }", ex.Message);
            return this.Unauthorized();
        }
    }
}