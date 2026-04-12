using Application.Abstractions.Training;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models;
using System.Security.Claims;

namespace Presentation.WebApp.Areas.Account.Controllers;

[Area("Account")]
[Authorize]
[Route("me/bookings")]
public class AccountBookingsController(
    ITrainingSessionService sessions,
    ITrainingSessionBookingService bookings) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "My Bookings";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var bookedIds = await bookings.GetBookingsForUserAsync(userId);
        var allSessions = await sessions.GetAllTrainingSessionsAsync();

        var model = allSessions
            .Where(s => bookedIds.Contains(s.Id))
            .Select(s =>
            {
                var bookedCount = s.Bookings.Count;
                var isFull = bookedCount >= s.MaxParticipants;

                return new TrainingSessionListItemViewModel(
                    Session: new TrainingSessionViewModel(
                        Id: s.Id,
                        Name: s.Name,
                        StartTime: s.StartTime,
                        EndTime: s.EndTime,
                        MaxParticipants: s.MaxParticipants
                    ),
                    IsBooked: true,
                    BookedCount: bookedCount,
                    IsFull: isFull
                );
            })
            .OrderBy(x => x.Session.StartTime)
            .ToList();

        return View(model);
    }
}
