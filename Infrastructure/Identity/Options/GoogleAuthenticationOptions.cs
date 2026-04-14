namespace Infrastructure.Identity.Options;

public class GoogleAuthenticationOptions
{
    public const string SectionName = "Authentication:Google";
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;

}
