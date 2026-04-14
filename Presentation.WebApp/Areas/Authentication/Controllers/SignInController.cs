using System.Security.Claims;
using Application.Abstractions.Identity;
using Application.Common.Outputs;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Presentation.WebApp.Areas.Authentication.Models;
using Presentation.WebApp.Services;

namespace Presentation.WebApp.Areas.Authentication.Controllers;

[Area("Authentication")]
public class SignInController(IAuthService authService, SignInManager<AppUser> signInManager) : Controller
{

    //Local signin

    [HttpGet("sign-in")]
    public IActionResult SignIn(string? returnUrl = null)
    {
        var redirectPath = AuthenticationRedirectService.GetRedirectPathWhenSignedIn(User);
        if (redirectPath is not null)
            return Redirect(redirectPath);

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("sign-in")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(SignInForm form, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(nameof(form.ErrorMessage), "Incorrect email address or password");
            ViewBag.ReturnUrl = returnUrl;
            return View(form);
        }

        try
        {
            var signedIn = await authService.SignInUserAsync(form.Email, form.Password, form.RememberMe);

            if (!signedIn.Succeeded)
            {
                var errorMessage = signedIn.ErrorType switch
                {
                    AuthErrorType.RequireTwoFactorAuth => "Requires two-factor authentication",
                    AuthErrorType.LockedOut => "This user is locked out. Please contact support.",
                    AuthErrorType.NotAllowed => "You are not allowed to login. Please contact support.",
                    _ => "Incorrect email address or password"
                };

                ModelState.AddModelError(nameof(form.ErrorMessage), errorMessage);
                ViewBag.ReturnUrl = returnUrl;
                return View(form);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            var redirectPath = AuthenticationRedirectService.GetRedirectPathWhenSignedIn(User);
            if (redirectPath is not null)
                return Redirect(redirectPath);

            return Redirect("/");
        }
        catch
        {
            return Redirect("/error");
        }
    }

    //Google signin

    [HttpPost("external-login")]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var callbackUrl = Url.Action(nameof(ExternalLoginCallback), "SignIn", new { area = "Authentication", returnUrl });
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);

        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        var safeReturnUrl =
            (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                ? returnUrl
                : null;

        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            TempData["ErrorMessage"] = $"External provider error: {remoteError}";
            return RedirectToAction(nameof(SignIn), new { returnUrl = safeReturnUrl });
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            TempData["ErrorMessage"] = "External login failed (missing external login info).";
            return RedirectToAction(nameof(SignIn), new { returnUrl = safeReturnUrl });
        }

        var result = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (!result.Succeeded)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] = "External login failed (email claim missing).";
                return RedirectToAction(nameof(SignIn), new { returnUrl = safeReturnUrl });
            }

            var user = await signInManager.UserManager.FindByEmailAsync(email.Trim());
            if (user is null)
            {
                user = AppUser.Create(email);
                user.ImageUrl = info.Principal.FindFirstValue("urn:google:picture");

                var created = await signInManager.UserManager.CreateAsync(user);
                if (!created.Succeeded)
                {
                    TempData["ErrorMessage"] = created.Errors.FirstOrDefault()?.Description ?? "Unable to create a local user for external login.";
                    return RedirectToAction(nameof(SignIn), new { returnUrl = safeReturnUrl });
                }
            }

            var addedLogin = await signInManager.UserManager.AddLoginAsync(
                user,
                new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName));

            if (!addedLogin.Succeeded)
            {
                TempData["ErrorMessage"] = addedLogin.Errors.FirstOrDefault()?.Description ?? "Unable to link external login.";
                return RedirectToAction(nameof(SignIn), new { returnUrl = safeReturnUrl });
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        if (!string.IsNullOrWhiteSpace(safeReturnUrl))
            return Redirect(safeReturnUrl);

        var signedInUser = await signInManager.UserManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (signedInUser is not null)
        {
            var principal = await signInManager.CreateUserPrincipalAsync(signedInUser);
            var redirectPath = AuthenticationRedirectService.GetRedirectPathWhenSignedIn(principal);
            return Redirect(redirectPath ?? "/");
        }

        return Redirect("/");
    }
}