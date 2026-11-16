
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrganiserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Dashboard()
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

            if (!int.TryParse(
                HttpContext.Session.GetString("UserId"),
                out int organiserId))
            {
                return Unauthorized();
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Retrieve events
            var eventsResponse = await client.GetAsync(
                "api/Events/get_events");

            if (!eventsResponse.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve your events.");
            }

            var allEvents = await eventsResponse.Content
                .ReadFromJsonAsync<List<Event>>(options);

            // Keep only this organiser's events
            var ownEvents = allEvents?
                .Where(e => e.OrganiserId == organiserId)
                .ToList() ?? new List<Event>();

            var ownEventIds = ownEvents
                .Select(e => e.EventId)
                .ToHashSet();

            // Retrieve categories
            var categoriesResponse = await client.GetAsync(
                "api/Categories/get_categories");

            if (!categoriesResponse.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve categories.");
            }

            var allCategories = await categoriesResponse.Content
                .ReadFromJsonAsync<List<Category>>(options);

            var ownCategories = allCategories?
                .Where(c => ownEventIds.Contains(c.EventId))
                .ToList() ?? new List<Category>();

            // Retrieve this organiser's enrolments
            var enrolmentsResponse = await client.GetAsync(
                "api/Enrolments/enrolments");

            if (!enrolmentsResponse.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve your enrolments.");
            }

            var enrolments = await enrolmentsResponse.Content
                .ReadFromJsonAsync<List<Enrolment>>(options);

            // Retrieve this organiser's results
            var resultsResponse = await client.GetAsync(
                "api/Results/get_results");

            if (!resultsResponse.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve your results.");
            }

            var results = await resultsResponse.Content
                .ReadFromJsonAsync<List<Result>>(options);

            // Build dashboard
            var dashboard = new OrganiserDashboardViewModel
            {
                Events = ownEvents,
                Categories = ownCategories ?? new List<Category>(),
                Enrolments = enrolments ?? new List<Enrolment>(),
                Results = results ?? new List<Result>()
            };

            return View(
                "~/Views/Organiser/Dashboard.cshtml",
                dashboard);
        }
    }
}
