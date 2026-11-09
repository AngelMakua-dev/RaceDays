using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    public class ParticipantController : Controller
    {
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (role != "Participant")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}