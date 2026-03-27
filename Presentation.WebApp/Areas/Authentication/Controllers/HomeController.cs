using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Presentation.WebApp.Areas.Authentication.Models;

namespace Presentation.WebApp.Areas.Authentication.Controllers;

[Area("Authentication")]
public class HomeController(IAuthService authService) : Controller
{
    [HttpGet("Registration/sign-up")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("Registration/sign-up")]
    public async Task<IActionResult> SignIn(SignUpForm form)
    {

        if(!ModelState.IsValid)
            return View(form);
        

        HttpContext.Session.SetString("reg_email", form.Email);

        return RedirectToAction(nameof(SetPassword));
    }

    [HttpPost("Registration/set-password")]
    public async Task<IActionResult> SetPassword(SetPasswordForm form)
    {
        var email = HttpContext.Session.GetString("reg_email");

        if (string.IsNullOrWhiteSpace(email))
            return RedirectToAction(nameof(Index));

        if (!ModelState.IsValid)
            return View(form);

        var created = await authService.SignUpUserAsync(email, form.Password);

        if (!created.Succeeded)
        {
            ModelState.AddModelError(nameof(form.ErrorMessage), created.ErrorMessage ?? "An error occurred while creating the user.");
            return View(form);
        }

        var result = await authService.SignInUserAsync(email, form.Password, false);

        if (!result.Succeeded)
        {
            return RedirectToAction(nameof(SignIn));
        }

        return Redirect("/me");
    }
}
