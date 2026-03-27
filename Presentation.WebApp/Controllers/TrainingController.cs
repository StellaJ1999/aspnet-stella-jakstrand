using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

[Route("training")]
public class TrainingController : Controller
{
    [HttpGet("group-training")]
    [AllowAnonymous]
    public IActionResult GroupTraining()
    {
        ViewData["Title"] = "Group Training";
        return View();
    }

    [HttpGet("personal-traineer")]
    [AllowAnonymous]
    public IActionResult PersonalTraineer()
    {
        ViewData["Title"] = "Personal Training";
        return View();
    }

    [HttpGet("online-coaching")]
    [AllowAnonymous]
    public IActionResult OnlineCoaching()
    {
        ViewData["Title"] = "Online Coaching";
        return View();
    }
}
