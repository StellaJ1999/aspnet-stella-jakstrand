using Application.Abstractions.Identity;
using Application.Common.Outputs;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Services;

public sealed class IdentityAuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager) : IAuthService
{

    public async Task<AuthResult> SignUpUserAsync(string email, string password, string? roleName = null)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return AuthResult.Failed("Email and password must be provided.");
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
            return AuthResult.Failed("User with this email already exists.");

        user = new AppUser
        {
            UserName = email.Trim(),
            Email = email.Trim()
        };

        var created = await userManager.CreateAsync(user, password);
        if (!created.Succeeded)
        {
            return AuthResult.Failed(created.Errors.FirstOrDefault()?.Description ?? "Unable to create user");
        }

        if (!string.IsNullOrWhiteSpace(roleName))
        {
            roleName = roleName.Trim();

            if (!await roleManager.RoleExistsAsync(roleName))
                return AuthResult.Failed($"Role '{roleName}' does not exist.");

            var addedToRole = await userManager.AddToRoleAsync(user, roleName);
            if (!addedToRole.Succeeded)
                return AuthResult.Failed(addedToRole.Errors.FirstOrDefault()?.Description ?? $"Unable to add user to role '{roleName}'.");
        }

        return AuthResult.Ok();
    }

    public async Task<AuthResult> SignInUserAsync(string email, string password, bool rememberMe)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return AuthResult.InvalidCredentials();

        var user = await userManager.FindByEmailAsync(email.Trim());
        if (user is null)
            return AuthResult.InvalidCredentials();

        var result = await signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
        if (result.IsLockedOut)
            return AuthResult.LockedOut();

        if (result.IsNotAllowed)
            return AuthResult.NotAllowed();

        if (result.RequiresTwoFactor)
            return AuthResult.RequireTwoFactorAuth();

        if (!result.Succeeded)
            return AuthResult.Failed();

        return AuthResult.Ok();
    }

    public Task SignOutUserAsync() => signInManager.SignOutAsync();
}
