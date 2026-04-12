using Application.Abstractions.Memberships;
using Domain.Memberships;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models;
using System.Security.Claims;

namespace Presentation.WebApp.Areas.Account.Controllers;

[Area("Account")]
[Authorize]
[Route("me/membership")]
public sealed class AccountMembershipController(IMembershipService memberships) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "My Membership";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var utcNow = DateTime.UtcNow;

        var active = await memberships.GetActiveSubscriptionForUserAsync(userId, utcNow);
        if (active is null)
            return View(new MyMembershipViewModel());

        var offers = await memberships.GetOffersAsync();
        var offer = offers.FirstOrDefault(x => x.Id == active.MembershipOfferId);

        return View(new MyMembershipViewModel
        {
            Active = new MyMembershipViewModel.ActiveMembershipViewModel(
                Tier: offer?.Tier ?? MembershipTier.Standard,
                MonthlyPrice: offer?.MonthlyPrice ?? 0m,
                DurationInMonths: active.DurationInMonths,
                StartUtc: active.StartUtc,
                EndUtc: active.EndUtc,
                Status: active.Status)
        });
    }

    [HttpPost("cancel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        await memberships.CancelActiveSubscriptionAsync(userId, DateTime.UtcNow);
        return RedirectToAction(nameof(Index));
    }
}