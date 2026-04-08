using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin")]
public class AdminController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "AdminMemberships");

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        return View();
    }
}
