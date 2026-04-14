using Application.Abstractions.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Areas.Authentication.Models;
using Presentation.WebApp.Services;

namespace Presentation.WebApp.Areas.Authentication.Controllers;

[Area("Authentication")]
[Route("registration")]
public class SignUpController(IAuthService authService) : Controller
{
    private const string RegEmailSessionKey = "reg_email";

    [HttpGet("sign-up")]
    public IActionResult SignUp(string? returnUrl = null)
    {
        var redirectPath = AuthenticationRedirectService.GetRedirectPathWhenSignedIn(User);
        if (redirectPath is not null)
            return Redirect(redirectPath);
       
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("sign-up")]
    [ValidateAntiForgeryToken]
    public IActionResult SignUp(SignUpForm form)
    {
        if (!ModelState.IsValid)
            return View(form);

        HttpContext.Session.SetString(RegEmailSessionKey, form.Email.Trim());
        return RedirectToAction(nameof(SetPassword));
    }

    [HttpGet("set-password")]
    public IActionResult SetPassword()
    {
        var redirectPath = AuthenticationRedirectService.GetRedirectPathWhenSignedIn(User);
        if (redirectPath is not null)
            return Redirect(redirectPath);

        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(RegEmailSessionKey)))
            return RedirectToAction(nameof(SignUp));

        return View();
    }

    [HttpPost("set-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetPasswordForm form)
    {
        var email = HttpContext.Session.GetString(RegEmailSessionKey);
        if (string.IsNullOrWhiteSpace(email))
            return RedirectToAction(nameof(SignUp));

        if (!ModelState.IsValid)
            return View(form);

        if (!string.Equals(form.Password, form.ConfirmPassword, StringComparison.Ordinal))
        {
            ModelState.AddModelError(nameof(form.ErrorMessage), "Passwords do not match.");
            return View(form);
        }

        var created = await authService.SignUpUserAsync(email, form.Password);
        if (!created.Succeeded)
        {
            ModelState.AddModelError(nameof(form.ErrorMessage), created.ErrorMessage ?? "An error occurred while creating the user.");
            return View(form);
        }

        var signedIn = await authService.SignInUserAsync(email, form.Password, rememberMe: false);
        if (!signedIn.Succeeded)
            return Redirect("/sign-in");

        HttpContext.Session.Remove(RegEmailSessionKey);

        var redirectPath = AuthenticationRedirectService.GetRedirectPathWhenSignedIn(User);
        return Redirect(redirectPath ?? "/");
    }
}