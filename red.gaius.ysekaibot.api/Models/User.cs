namespace Red.Gaius.YsekaiBot.Api.Models;

/// <summary>
/// User data access object.
/// </summary>
public class User
{
    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    public User()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class from Discord user data.
    /// </summary>
    /// <param name="discordUserData">Discord user data.</param>
    public User(Dictionary<string, object> discordUserData)
    {
        this.AvatarHash = discordUserData["avatar"].ToString();
        this.Discriminator = discordUserData["discriminator"].ToString();
        this.Email = discordUserData["email"].ToString();
        this.Id = discordUserData["id"].ToString();
        this.Locale = discordUserData["locale"].ToString();
        this.Username = discordUserData["username"].ToString();
        this.Verified = bool.Parse(discordUserData["verified"].ToString()!);
    }

    /// <summary>
    /// Gets or sets the user's id.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the user's username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets the user's discord username.
    /// </summary>
    public string DiscordUsername
    {
        get
        {
            return this.Username + "#" + this.Discriminator;
        }
    }

    /// <summary>
    /// Gets or sets the user's avatar hash.
    /// </summary>
    public string? AvatarHash { get; set; }

    /// <summary>
    /// Gets the user's avatar url.
    /// </summary>
    public string AvatarUrl
    {
        get
        {
            if (this.AvatarHash!.StartsWith("a_"))
            {
                return "https://cdn.discordapp.com/avatars/" + this.Id + "/" + this.AvatarHash + ".gif";
            }
            else
            {
                return "https://cdn.discordapp.com/avatars/" + this.Id + "/" + this.AvatarHash + ".jpg";
            }
        }
    }

    /// <summary>
    /// Gets or sets the user's discriminator.
    /// </summary>
    public string? Discriminator { get; set; }

    /// <summary>
    /// Gets or sets the user's locale.
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is verified.
    /// </summary>
    public bool Verified { get; set; }
}