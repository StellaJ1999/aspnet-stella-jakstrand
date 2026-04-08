
using Application.Abstractions.Identity;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Data;

internal class IdentityInitializer()
{
    public static async Task InitilizeDefaultRolesAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new List<string>()
        {
            "Admin",
            "Member"
        };

        try
        {
            foreach (var role in roles)
            {
                if (!string.IsNullOrWhiteSpace(role) && !await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new(role));
            }
        }
        catch { }
    }

    public static async Task InitilizeDefaultAdminAccountsAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var admins = new List<string>()
        {
            "admin@domain.local",
        };

        try
        {
            if (!await userManager.Users.AnyAsync())
            {
                var defaultPassword = "BytMig123!";
                var defaultRoleName = "Admin";

                if (!string.IsNullOrWhiteSpace(defaultPassword))
                {
                    foreach (var admin in admins)
                    {
                        var user = AppUser.Create(admin);
                        user.EmailConfirmed = true;

                        var created = await userManager.CreateAsync(user, defaultPassword);

                        if (created.Succeeded && await roleManager.RoleExistsAsync(defaultRoleName))
                            await userManager.AddToRoleAsync(user, defaultRoleName);
                    }

                }
            }
        }
        catch { }
    }
}
