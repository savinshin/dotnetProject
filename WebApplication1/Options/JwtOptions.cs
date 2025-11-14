namespace WebApplication1.Options;

public class JwtOptions
{
    public string Issuer { get; set; } = "WebApp";
    public string Audience { get; set; } = "WebApp";
    public string Key { get; set; } = "dev_secret_key_change_me_very_long_123!";
    public int ExpiresMinutes { get; set; } = 60;
}
