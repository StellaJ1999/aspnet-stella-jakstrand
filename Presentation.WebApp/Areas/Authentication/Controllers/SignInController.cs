using Application.Abstractions.Identity;
using Application.Common.Outputs;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Areas.Authentication.Models;
using Presentation.WebApp.Services;

namespace Presentation.WebApp.Areas.Authentication.Controllers;

[Area("Authentication")]
public class SignInController(IAuthService authService) : Controller
{

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

}