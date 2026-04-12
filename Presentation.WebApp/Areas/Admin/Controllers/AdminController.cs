using Application.Abstractions.Training;
using Application.Training.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Common;
using Presentation.WebApp.Areas.Admin.Models;

namespace Presentation.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("[area]/[controller]")]
public class AdminController : Controller
{
    private readonly ITrainingSessionService _sessions;

    public AdminController(ITrainingSessionService sessions)
    {
        _sessions = sessions;
    }

    public IActionResult Index() => RedirectToAction("Index", "AdminMemberships");

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet("schedule")]
    public async Task<IActionResult> Schedule()
    {
        ViewData["Title"] = "Schedule";
        var sessions = await _sessions.GetAllTrainingSessionsAsync();

        var model = sessions
            .Select(s => new TrainingSessionScheduleItemViewModel(
                Id: s.Id,
                Name: s.Name,
                StartTime: s.StartTime,
                EndTime: s.EndTime,
                MaxParticipants: s.MaxParticipants
            ))
            .ToList();

        return View(model);
    }

    [HttpGet("schedule/create")]
    public IActionResult CreateTrainingSession()
    {
        ViewData["Title"] = "Create session";
        return View("TrainingSessionForm", new TrainingSessionForm());
    }

    [HttpPost("schedule/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTrainingSession(TrainingSessionForm form)
    {
        ViewData["Title"] = "Create session";

        if (!ModelState.IsValid)
            return View("TrainingSessionForm", form);

        var input = new TrainingSessionInput(
            form.Name,
            form.StartTime,
            form.EndTime,
            form.MaxParticipants
        );

        try
        {
            var ok = await _sessions.CreateTrainingSessionAsync(input);
            TempData["Message"] = ok ? "Session created." : "Could not create session.";
            return RedirectToAction(nameof(Schedule));
        }
        catch (DomainException ex)
        {
            // Surface domain invariant violations back into the form.
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("TrainingSessionForm", form);
        }
    }

    [HttpGet("schedule/{id:guid}/edit")]
    public async Task<IActionResult> EditTrainingSession(Guid id)
    {
        ViewData["Title"] = "Edit session";

        var session = await _sessions.GetTrainingSessionByIdAsync(id);
        if (session is null)
            return NotFound();

        var form = new TrainingSessionForm
        {
            Name = session.Name,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            MaxParticipants = session.MaxParticipants
        };

        return View("EditTrainingSessionForm", form);
    }

    [HttpPost("schedule/{id:guid}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTrainingSession(Guid id, TrainingSessionForm form)
    {
        ViewData["Title"] = "Edit session";

        if (!ModelState.IsValid)
            return View("EditTrainingSessionForm", form);

        var input = new TrainingSessionInput(
            form.Name,
            form.StartTime,
            form.EndTime,
            form.MaxParticipants
        );

        try
        {
            var ok = await _sessions.UpdateTrainingSessionAsync(id, input);
            TempData["Message"] = ok ? "Session updated." : "Could not update session.";
            return RedirectToAction(nameof(Schedule));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("EditTrainingSessionForm", form);
        }
    }

    [HttpPost("schedule/{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTrainingSession(Guid id)
    {
        var ok = await _sessions.DeleteTrainingSessionAsync(id);
        TempData["Message"] = ok ? "Session deleted." : "Could not delete session.";

        return RedirectToAction(nameof(Schedule));
    }
}
