using Application.Abstractions.Support;
using Application.Support.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Common;
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

        // Anropar Application-lagret via service interfacet.
        // Controllern har ingen kunskap om hur det är implementerat, bara att det finns en metod som tar ett input-objekt och returnerar en bool.
        try
        {
            var ok = await _contactRequestService.CreateContactRequestAsync(input);

            // Detta är den text visas under knappen efter att POST:en är klar.
            TempData["Message"] = ok ? "Contact request sent." : "Contact request failed to send.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            // Show domain validation error under the submit button (uses TempData pattern in the partial).
            TempData["Message"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}