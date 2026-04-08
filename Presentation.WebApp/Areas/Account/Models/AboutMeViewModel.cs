namespace Presentation.WebApp.Areas.Account.Models;

public class AboutMeViewModel
{
    public AboutMeForm AboutMeForm { get; set; } = new();
    public string? ProfileImageUrl { get; set; }
}
