using Microsoft.AspNetCore.Mvc;
using Red.Gaius.YsekaiBot.Services;

namespace Red.Gaius.YsekaiBot.Api.Controllers;

/// <summary>
/// Authentication via Discord controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AuthController : ControllerBase
{
    private readonly DiscordAuth auth;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="auth">Discord authentication service.</param>
    public AuthController(DiscordAuth auth)
    {
        this.auth = auth;
    }

    /// <summary>
    /// Authentication method.
    /// </summary>
    /// <remarks>This endpoint authenticates users via Discord OAUTH2.</remarks>
    /// <param name="state">Url to return to after authentication.</param>
    /// <param name="code">Discord authentication code.</param>
    [HttpGet(Name = "Authenticate")]
    public void Get(string? state = "", string? code = "")
    {
        if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(code))
        {
            var returnUrl = this.Request.Headers.Referer;
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = state;
            }

            this.Response.Redirect(this.auth.GetAuthCodeUrl(
                returnUrl,
                "https://" + this.Request.Host.ToString() + "/api/Auth"));
        }
        else
        {
            var token = this.auth.GetAccessToken(code, "https://" + this.Request.Host.ToString() + "/api/Auth");
            this.Response.Cookies.Append("__Secure-Access-Token", token["access_token"].ToString()!, new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddSeconds(double.Parse(token["expires_in"].ToString()!)),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
            });
            this.Response.Redirect(state);
        }
    }
}