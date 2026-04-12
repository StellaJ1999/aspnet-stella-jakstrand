using Application.Abstractions.Training;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models;
using System.Security.Claims;

namespace Presentation.WebApp.Controllers;

[Route("training")]
public class TrainingController : Controller
{
    private readonly ITrainingSessionService? _sessions;
    private readonly ITrainingSessionBookingService? _bookings;

    public TrainingController(ITrainingSessionService sessions, ITrainingSessionBookingService bookings)
    {
        _sessions = sessions;
        _bookings = bookings;
    }

    [HttpGet("")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Training";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var sessions = await _sessions.GetAllTrainingSessionsAsync();
        var bookedIds = await _bookings.GetBookingsForUserAsync(userId);

        var model = sessions
            .Select(s =>
            {
                var bookedCount = s.Bookings.Count;
                var isFull = bookedCount >= s.MaxParticipants;
                var isBooked = bookedIds.Contains(s.Id);

                return new TrainingSessionListItemViewModel(
                    Session: new TrainingSessionViewModel(
                        Id: s.Id,
                        Name: s.Name,
                        StartTime: s.StartTime,
                        EndTime: s.EndTime,
                        MaxParticipants: s.MaxParticipants
                    ),
                    IsBooked: isBooked,
                    BookedCount: bookedCount,
                    IsFull: isFull
                );
            })
            .ToList();

        return View(model);
    }

    [HttpPost("book/{id:guid}")]
    [Authorize(Roles = "Admin,User")]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Book(Guid id)
    {
        // För att boka en session måste vi veta vem som bokar, så vi hämtar userId från claims.
        // Om det inte finns någon userId så uppmanar vi användaren att logga in.

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var ok = await _bookings.BookAsync(id, userId);
        TempData["Message"] = ok ? "Booked." : "Could not book (maybe full or already booked).";

        return RedirectToAction(nameof(Index));
    }

     [HttpPost("cancel/{id:guid}")]
     [Authorize(Roles = "Admin,User")]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> Cancel(Guid id)
     {
         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
         if (string.IsNullOrWhiteSpace(userId))
             return Challenge();

        var ok = await _bookings.CancelBookingAsync(id, userId);
        TempData["Message"] = ok ? "Booking canceled." : "Could not cancel booking.";

        return RedirectToAction(nameof(Index));
    }
}
