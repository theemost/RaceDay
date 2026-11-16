using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Text.Json;
using System.Security.Claims;
using System.Net.Http.Json;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var url = $"api/Authentication/login?username={Uri.EscapeDataString(model.Username)}&password={Uri.EscapeDataString(model.Password)}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var token = document.RootElement
                .GetProperty("token")
                .GetString();

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "Login failed.");
                return View(model);
            }


            var tokenParts = token.Split('.');

            var payload = tokenParts[1]
                .Replace('-', '+')
                .Replace('_', '/');

            payload = payload.PadRight(
                payload.Length + (4 - payload.Length % 4) % 4,
                '=');

            var payloadBytes = Convert.FromBase64String(payload);

            using var tokenDocument = JsonDocument.Parse(payloadBytes);

            var claims = tokenDocument.RootElement;

            var role = claims.TryGetProperty(ClaimTypes.Role, out var roleClaim)
                ? roleClaim.GetString()
                : null;

            var userId = claims.TryGetProperty(ClaimTypes.NameIdentifier, out var idClaim)
                ? idClaim.GetString()
                : null;

            var firstName = claims.TryGetProperty("firstName", out var firstNameClaim)
                ? firstNameClaim.GetString()
                : null;

            var lastName = claims.TryGetProperty("lastName", out var lastNameClaim)
                ? lastNameClaim.GetString()
                : null;
            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString("Username", model.Username);
            HttpContext.Session.SetString("Role", role ?? "");
            HttpContext.Session.SetString("UserId", userId ?? "");
            HttpContext.Session.SetString("FirstName", firstName ?? "");
            HttpContext.Session.SetString("LastName", lastName ?? "");

            return RedirectToAction("Index", "Events");
        }

        // GET: /Account/Register
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
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var registrationData = new
            {
                firstName = model.FirstName,
                lastName = model.LastName,
                emailAddress = model.EmailAddress,
                username = model.Username,
                password = model.Password,
                role = model.Role
            };

            // Register the new user
            var response = await client.PostAsJsonAsync(
                "api/Authentication/register/",
                registrationData);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();

                try
                {
                    using var errorDocument = JsonDocument.Parse(errorResponse);
                    errorResponse = errorDocument.RootElement
                        .GetProperty("message").GetString() ?? errorResponse;
                }
                catch (JsonException)
                {
                    // Keep the original error response.
                }

                ModelState.AddModelError("", errorResponse);
                return View(model);
            }

            // Automatically log in the newly registered user
            var loginUrl =
                $"api/Authentication/login?username={Uri.EscapeDataString(model.Username)}" +
                $"&password={Uri.EscapeDataString(model.Password)}";

            var loginResponse = await client.GetAsync(loginUrl);

            if (!loginResponse.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registration successful. Please log in.";
                return RedirectToAction("Login");
            }

            var json = await loginResponse.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var token = document.RootElement
                .GetProperty("token")
                .GetString();

            if (string.IsNullOrEmpty(token))
            {
                TempData["Success"] = "Registration successful. Please log in.";
                return RedirectToAction("Login");
            }

            // Read the JWT claims
            var tokenParts = token.Split('.');

            var payload = tokenParts[1]
                .Replace('-', '+')
                .Replace('_', '/');

            payload = payload.PadRight(
                payload.Length + (4 - payload.Length % 4) % 4,
                '=');

            var payloadBytes = Convert.FromBase64String(payload);

            using var tokenDocument = JsonDocument.Parse(payloadBytes);

            var claims = tokenDocument.RootElement;

            var role = claims.TryGetProperty(ClaimTypes.Role, out var roleClaim)
                ? roleClaim.GetString()
                : null;

            var userId = claims.TryGetProperty(
                ClaimTypes.NameIdentifier, out var idClaim)
                ? idClaim.GetString()
                : null;

            var firstName = claims.TryGetProperty("firstName", out var firstNameClaim)
                ? firstNameClaim.GetString()
                : null;

            var lastName = claims.TryGetProperty("lastName", out var lastNameClaim)
                ? lastNameClaim.GetString()
                : null;

            // Store the user's details in the session
            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString("Username", model.Username);
            HttpContext.Session.SetString("Role", role ?? "");
            HttpContext.Session.SetString("UserId", userId ?? "");
            HttpContext.Session.SetString("FirstName", firstName ?? "");
            HttpContext.Session.SetString("LastName", lastName ?? "");

            return RedirectToAction("Index", "Events");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }


    }
}
