using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    public class AuthController : Controller
    {
        // Display the login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}