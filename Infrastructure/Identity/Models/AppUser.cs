using Microsoft.AspNetCore.Identity;
using System.Net.NetworkInformation;


namespace Infrastructure.Identity.Models;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }


    public static AppUser Create(string email)
    {
        return new AppUser
        {
            UserName = email.Trim().ToLowerInvariant(),
            Email = email.Trim().ToLowerInvariant()
        };
    }

    public static AppUser UpdateDetails(AppUser user, string? firstName, string? lastName, string? imageUrl, string? phoneNumber)
    {
        if (user.FirstName != firstName)
            user.FirstName = firstName;

        if (user.LastName != lastName)
            user.LastName = lastName;

        if (user.ImageUrl != imageUrl)
            user.ImageUrl = imageUrl;

        if (user.PhoneNumber != phoneNumber)
            user.PhoneNumber = phoneNumber;

        return user;
    }
}
