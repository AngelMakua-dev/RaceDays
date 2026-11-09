using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using RaceDay.MVC.ViewModels;

namespace RaceDay.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public AuthController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PostAsync<
                    LoginViewModel,
                    LoginResponseViewModel>(
                    "api/Auth/login", model);

                if (response == null ||
                    string.IsNullOrWhiteSpace(response.Token))
                {
                    ModelState.AddModelError("", "Login failed.");
                    return View(model);
                }

                HttpContext.Session.SetString("Token", response.Token);
                HttpContext.Session.SetString("UserId", response.UserId.ToString());
                HttpContext.Session.SetString("FirstName", response.FirstName);
                HttpContext.Session.SetString("LastName", response.LastName);
                HttpContext.Session.SetString("Email", response.Email);
                HttpContext.Session.SetString("Role", response.Role);

                TempData["Success"] = "Login successful!";

                if (response.Role == "Participant")
                {
                    return RedirectToAction("Index", "Participant");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to log in. Please check your details and try again.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var response = await _apiService.PostForResponseAsync(
                    "api/Auth/register", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] =
                        "Account created successfully! Please log in.";

                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(
                    "",
                    "Registration failed. Please check your details.");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to connect to the RaceDay API.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["Success"] = "You have successfully logged out.";

            return RedirectToAction("Login");
        }
    }
}