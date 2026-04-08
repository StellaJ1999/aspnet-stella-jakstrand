using System.Security.Claims;

namespace Presentation.WebApp.Services;

public static class AuthenticationRedirectService
{
    public static string? GetRedirectPathWhenSignedIn(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            return null;

        if (user.IsInRole("Admin"))
            return "/admin";

        if (user.IsInRole("Member"))
            return "/me";

        return "/";
    }
}