using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

[Route("error")]
public sealed class ErrorController : Controller
{
    [HttpGet("{statusCode:int}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Status(int statusCode)
    {
        Response.StatusCode = statusCode;

        return statusCode switch
        {
            StatusCodes.Status404NotFound => View("NotFound"),
            _ => View("StatusCode", statusCode)
        };
    }
}