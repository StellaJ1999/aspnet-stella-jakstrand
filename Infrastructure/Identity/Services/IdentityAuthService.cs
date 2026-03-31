using Application.Abstractions.Authentication;
using Application.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Services;

public sealed class IdentityAuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : IAuthService
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

        return AuthResult.Ok();
    }

    public async Task<AuthResult> SignInUserAsync(string email, string password, bool rememberMe)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return AuthResult.Failed("Email and password must be provided.");
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
            return AuthResult.Failed("Invalid email or password.");


        var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, false);

        if (!result.Succeeded)
            return AuthResult.Failed("Invalid email or password.");
        if (result.IsLockedOut)
            return AuthResult.Failed("User account is locked out. Please contact support.");

        if (result.Succeeded)
            return AuthResult.Ok();

        return AuthResult.Failed("An unknown error occurred during sign-in.");
    }

    public Task SignOutUserAsync() => signInManager.SignOutAsync();
}
