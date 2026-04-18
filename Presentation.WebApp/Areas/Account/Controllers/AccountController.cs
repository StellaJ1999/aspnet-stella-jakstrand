using Application.Abstractions.Identity;
using Application.Common.Outputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Areas.Account.Models;
using Presentation.WebApp.Services;
using System.Security.Claims;

namespace Presentation.WebApp.Areas.Account.Controllers;


[Area("Account")]
[Route("me")]
[Authorize]
public class AccountController(IAuthService authService, IUserService userService) : Controller
{
    public IActionResult Index() => RedirectToAction(nameof(AboutMe));

    [HttpGet("about-me")]
    public async Task<IActionResult> AboutMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(userId))
        {
            var viewModel = new AboutMeViewModel();

            var result = await userService.GetUserDetailsAsync(userId);
            if (result.Details is not null)
            {
                viewModel.AboutMeForm = new AboutMeForm
                {
                    FirstName = result.Details.FirstName ?? string.Empty,
                    LastName = result.Details.LastName ?? string.Empty,
                    Email = result.Details.Email ?? string.Empty,
                    PhoneNumber = result.Details.PhoneNumber ?? string.Empty
                };
                viewModel.ProfileImageUrl = result.Details.ImageUrl;
            }

            return View(viewModel);
        }

        await authService.SignOutUserAsync();
        return Redirect("/");
    }

    [HttpPost("about-me")]
    public async Task<IActionResult> AboutMe(AboutMeViewModel viewModel)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction(nameof(SignOut));

        var userResult = await userService.GetUserDetailsAsync(userId);
        if (userResult.Details is null)
            return RedirectToAction(nameof(SignOut));

        if (!ModelState.IsValid)
            return View(viewModel);

        var imageUrl = userResult.Details.ImageUrl;

        if (viewModel.AboutMeForm.ProfileImage is not null && viewModel.AboutMeForm.ProfileImage.Length > 0)
        {
            imageUrl = await SaveProfileImageAsync(viewModel.AboutMeForm.ProfileImage);
        }

        var userDetails = new UserDetails
            (
                userId,
                viewModel.AboutMeForm.Email,
                viewModel.AboutMeForm.FirstName,
                viewModel.AboutMeForm.LastName,
                viewModel.AboutMeForm.PhoneNumber,
                imageUrl
            );

        var result = await userService.UpdateUserDetailsAsync(userDetails);
        if (!result.Succeeded)
        {
            viewModel.ProfileImageUrl = imageUrl ?? "~/images/profile-image-avatar.png";
            return View(viewModel);
        }

        return RedirectToAction(nameof(AboutMe));

    }

    [HttpPost("remove-account")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction(nameof(SignOut));

        var result = await userService.DeleteUserAsync(userId);
        if (!result.Succeeded)
            return RedirectToAction(nameof(AboutMe));

        await authService.SignOutUserAsync();
        return Redirect("/");
    }


    #region sign-out

    [HttpGet("sign-out")]
    public new async Task<IActionResult> SignOut()
    {
        await authService.SignOutUserAsync();
        return Redirect("/");
    }

    #endregion



    private static async Task<string> SaveProfileImageAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/profiles/{fileName}";
    }
}
