using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // View Profile
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(
                "api/UserProfile/profile");

            if (!response.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve your profile.");
            }

            var profile = await response.Content
                .ReadFromJsonAsync<ProfileViewModel>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (profile == null)
            {
                return NotFound("Profile not found.");
            }

            return View(profile);
        }

        // Update Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (string.IsNullOrEmpty(
                HttpContext.Session.GetString("JWToken")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                model.Role = HttpContext.Session.GetString("Role") ?? "";
                return View(model);
            }

            var token = HttpContext.Session.GetString("JWToken");

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var url = "api/UserProfile/update_profile" +
                      $"?firstName={Uri.EscapeDataString(model.FirstName)}" +
                      $"&lastName={Uri.EscapeDataString(model.LastName)}" +
                      $"&emailAddress={Uri.EscapeDataString(model.EmailAddress)}" +
                      $"&username={Uri.EscapeDataString(model.Username)}";

            var response = await client.PutAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to update your profile. Your username or email may already be in use.");

                model.Role = HttpContext.Session.GetString("Role") ?? "";

                return View(model);
            }

            // Update the session with the new information
            HttpContext.Session.SetString("FirstName", model.FirstName);
            HttpContext.Session.SetString("LastName", model.LastName);
            HttpContext.Session.SetString("Username", model.Username);

            TempData["Success"] = "Your profile has been updated successfully.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (string.IsNullOrEmpty(
                HttpContext.Session.GetString("JWToken")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
    ChangePasswordViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.CurrentPassword == model.NewPassword)
            {
                ModelState.AddModelError(
                    "",
                    "Your new password must be different from your current password.");

                return View(model);
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                currentPassword = model.CurrentPassword,
                newPassword = model.NewPassword,
                confirmPassword = model.ConfirmPassword
            };

            var response = await client.PutAsJsonAsync(
                "api/UserProfile/change_password",
                request);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to change password. Please check your current password.");

                return View(model);
            }

            TempData["Success"] = "Your password has been changed successfully.";

            return RedirectToAction("ChangePassword");
        }
    }
}