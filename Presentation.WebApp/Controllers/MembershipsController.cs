using Application.Abstractions.Memberships;
using Domain.Memberships;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models;
using System.Security.Claims;

namespace Presentation.WebApp.Controllers;

[Route("memberships")]
public class MembershipsController(IMembershipService memberships) : Controller
{
    [HttpGet("")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        ViewData["Title"] = "Our Memberships";
        return View();
    }

    [HttpGet("join")]
    [Authorize]
    public async Task<IActionResult> Join(string? plan = null)
    {
        ViewData["Title"] = "Join Membership";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var utcNow = DateTime.UtcNow;

        var existing = await memberships.GetActiveSubscriptionForUserAsync(userId, utcNow);
        if (existing is not null)
            return RedirectToAction("Index", "AccountMembership", new { area = "Account" });

        var offers = await memberships.GetOffersAsync();

        var vm = new MembershipJoinViewModel
        {
            Offers = offers.Select(x => new MembershipJoinViewModel.OfferOption(x.Tier, x.MonthlyPrice)).ToList()
        };

        if (!string.IsNullOrWhiteSpace(plan) && Enum.TryParse<MembershipTier>(plan, ignoreCase: true, out var tier))
            vm.Tier = tier;

        return View(vm);
    }

    [HttpPost("join")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(MembershipJoinViewModel vm)
    {
        ViewData["Title"] = "Join Membership";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var offers = await memberships.GetOffersAsync();
        vm = new MembershipJoinViewModel
        {
            Offers = offers.Select(x => new MembershipJoinViewModel.OfferOption(x.Tier, x.MonthlyPrice)).ToList(),
            Tier = vm.Tier,
            DurationInMonths = vm.DurationInMonths
        };

        if (vm.DurationInMonths < 1 || vm.DurationInMonths > 24)
        {
            vm.ErrorMessage = "Please select a duration between 1 and 24 months.";
            return View(vm);
        }

        var created = await memberships.CreateSubscriptionAsync(userId, vm.Tier, vm.DurationInMonths, DateTime.UtcNow);
        if (!created)
        {
            vm.ErrorMessage = "Unable to create membership. If you already have an active membership, cancel it first.";
            return View(vm);
        }

        return RedirectToAction("Index", "AccountMembership", new { area = "Account" });
    }
}
