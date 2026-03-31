using Application.Abstractions.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.CustomerService;

namespace Presentation.WebApp.Controllers;

[Route("support")]
public class SupportController : Controller
{
    private readonly IContactRequestService _contactRequestService;

    public SupportController(IContactRequestService contactRequestService)
    {
        // Controller pratar med Application via ett interface, inte med DbContext direkt.
        _contactRequestService = contactRequestService;
    }

    [HttpGet("")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        ViewData["Title"] = "Customer Service";
        return View(new ContactForm());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactForm form, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(form);

        // Input-objektet är Application-lagrets use-case input.
        // ContactRequestService sätter Id och CreatedAt och anropar sedan repositoryt i Infrastructure som sparar via EF Core.
        var input = new ContactRequestInput(
            form.FirstName,
            form.LastName,
            form.Email,
            form.Phone,
            form.Message
        );

        var ok = await _contactRequestService.CreateContactRequestAsync(input);

        // Detta är den text visas under knappen efter att POST:en är klar.
        TempData["Message"] = ok ? "Contact request sent." : "Contact request failed to send.";

        return RedirectToAction(nameof(Index));
    }
}