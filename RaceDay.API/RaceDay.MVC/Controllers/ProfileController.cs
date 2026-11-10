using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using RaceDay.MVC.ViewModels;

namespace RaceDay.MVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public ProfileController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var profile = await _apiService.GetWithTokenAsync<ProfileViewModel>(
                    "api/Profile/me", token);

                if (profile == null)
                {
                    TempData["Error"] = "Unable to load your profile.";
                    return View(new ProfileViewModel());
                }

                return View(profile);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to retrieve your profile from the API.";
                return View(new ProfileViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var profile = await _apiService.GetWithTokenAsync<ProfileViewModel>(
                    "api/Profile/me", token);

                if (profile == null)
                {
                    TempData["Error"] = "Unable to load your profile.";
                    return RedirectToAction(nameof(Index));
                }

                return View(profile);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to retrieve your profile.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await _apiService.PutWithTokenAsync<ProfileViewModel, ProfileViewModel>(
                    "api/Profile/me", model, token);

                if (response != null)
                {
                    TempData["Success"] = "Your profile has been updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Unable to update your profile.");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "The profile update could not be completed.");
            }

            return View(model);
        }
    }
}