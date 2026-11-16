using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class EventTypesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EventTypesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Display existing event types
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Index", "Events");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/EventTypes");

            if (!response.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve event types.");
            }

            var eventTypes = await response.Content
                .ReadFromJsonAsync<List<EventType>>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(eventTypes ?? new List<EventType>());
        }

        // Add a new event type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string eventTypeName)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Index", "Events");
            }

            if (string.IsNullOrWhiteSpace(eventTypeName))
            {
                TempData["Error"] = "Please enter an event type name.";
                return RedirectToAction("Index");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var url = "api/EventTypes?eventTypeName=" +
                      Uri.EscapeDataString(eventTypeName.Trim());

            var response = await client.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                TempData["Error"] =
                    $"Unable to add event type: {error}";

                return RedirectToAction("Index");
            }

            TempData["Success"] = "Event type added successfully.";

            return RedirectToAction("Index");
        }
    }
}