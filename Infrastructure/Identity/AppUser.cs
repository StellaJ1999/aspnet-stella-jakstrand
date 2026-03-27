
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;
public class AppUser : IdentityUser {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public static AppUser Create(string email) => new()
    {
        UserName = email.Trim().ToLowerInvariant(),
        Email = email.Trim().ToLowerInvariant()
    };
}
