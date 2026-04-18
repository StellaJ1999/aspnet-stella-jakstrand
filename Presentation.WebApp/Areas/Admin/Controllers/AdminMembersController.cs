using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.WebApp.Areas.Admin.Models;

namespace Presentation.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("[area]/[controller]")]
public class AdminMembersController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminMembersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("")]

    // /Admin/AdminMembers
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Members";

        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync();

        var members = new List<AdminMemberListItemViewModel>();
        foreach (var user in users)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isMember = await _userManager.IsInRoleAsync(user, "Member");

            members.Add(new AdminMemberListItemViewModel(
                user.Id,
                user.Email ?? string.Empty,
                user.FirstName,
                user.LastName,
                isAdmin,
                isMember));
        }

        return View(new AdminMembersListViewModel(members));
    }

    [HttpPost("update-role")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(string id, bool isAdmin)
    {
        if (string.IsNullOrWhiteSpace(id))
            return RedirectToAction(nameof(Index));

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return RedirectToAction(nameof(Index));

        await EnsureRoleExistsAsync("Admin");
        await EnsureRoleExistsAsync("Member");

        if (isAdmin)
        {
            await AddRoleIfMissingAsync(user, "Admin");
            await RemoveRoleIfPresentAsync(user, "Member");
        }
        else
        {
            await AddRoleIfMissingAsync(user, "Member");
            await RemoveRoleIfPresentAsync(user, "Admin");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return RedirectToAction(nameof(Index));

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return RedirectToAction(nameof(Index));

        await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Index));
    }

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            await _roleManager.CreateAsync(new IdentityRole(roleName));
    }

    private async Task AddRoleIfMissingAsync(AppUser user, string roleName)
    {
        if (!await _userManager.IsInRoleAsync(user, roleName))
            await _userManager.AddToRoleAsync(user, roleName);
    }

    private async Task RemoveRoleIfPresentAsync(AppUser user, string roleName)
    {
        if (await _userManager.IsInRoleAsync(user, roleName))
            await _userManager.RemoveFromRoleAsync(user, roleName);
    }
}