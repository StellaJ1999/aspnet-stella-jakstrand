using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin/memberships")]
public class AdminMembershipsController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
