using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers.Admin
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
