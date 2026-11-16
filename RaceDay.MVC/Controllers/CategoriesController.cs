using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RaceDay.MVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int eventId)
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
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            // Retrieve the selected event
            var eventResponse = await client.GetAsync(
                $"api/Events/get_event/{eventId}");

            if (!eventResponse.IsSuccessStatusCode)
            {
                return NotFound("Event not found.");
            }

            var eventDetails = await eventResponse.Content
                .ReadFromJsonAsync<Event>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (eventDetails == null)
            {
                return NotFound();
            }

            // Verify that the event belongs to the logged-in organiser
            int.TryParse(
                HttpContext.Session.GetString("UserId"),
                out int organiserId);

            if (eventDetails.OrganiserId != organiserId)
            {
                return Forbid();
            }

            // Retrieve categories belonging to this event
            var response = await client.GetAsync(
                $"api/Categories/event/{eventId}");

            if (!response.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve event categories.");
            }

            var categories = await response.Content
                .ReadFromJsonAsync<List<Category>>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            ViewBag.EventId = eventId;
            ViewBag.EventName = eventDetails.EventName;

            return View(categories ?? new List<Category>());
        }

        [HttpGet]
        public async Task<IActionResult> Create(int eventId)
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
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            // Retrieve the selected event
            var response = await client.GetAsync(
                $"api/Events/get_event/{eventId}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound("Event not found.");
            }

            var selectedEvent = await response.Content
                .ReadFromJsonAsync<Event>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (selectedEvent == null)
            {
                return NotFound();
            }

            int.TryParse(
                HttpContext.Session.GetString("UserId"),
                out int organiserId);

            if (selectedEvent.OrganiserId != organiserId)
            {
                return Forbid();
            }

            ViewBag.Events = new SelectList(
                new List<Event> { selectedEvent },
                "EventId",
                "EventName",
                eventId);

            return View(new CreateCategoryViewModel
            {
                EventId = eventId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var eventClient = _httpClientFactory.CreateClient("RaceDayAPI");

                var eventsResponse = await eventClient.GetAsync(
                    "api/Events/get_events");

                if (eventsResponse.IsSuccessStatusCode)
                {
                    var events = await eventsResponse.Content
                        .ReadFromJsonAsync<List<Event>>(
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                    int.TryParse(
                        HttpContext.Session.GetString("UserId"),
                        out int organiserId);

                    ViewBag.Events = new SelectList(
                        events?.Where(e => e.OrganiserId == organiserId).ToList(),
                        "EventId",
                        "EventName");
                }

                return View(model);
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.PostAsync(
                $"api/Categories?categoryName={Uri.EscapeDataString(model.CategoryName)}&eventId={model.EventId}",
                null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Content(
                    $"Unable to create category.\nStatus: {response.StatusCode}\nError: {error}");
            }

            TempData["Success"] = "Category created successfully.";

            return RedirectToAction("Index", new { eventId = model.EventId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var categoryResponse = await client.GetAsync(
                $"api/Categories/get_category/{id}");

            if (!categoryResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var category = await categoryResponse.Content
                .ReadFromJsonAsync<Category>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (category == null)
            {
                return NotFound();
            }

            var eventsResponse = await client.GetAsync(
                "api/Events/get_events");

            if (!eventsResponse.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve events.");
            }

            var events = await eventsResponse.Content
                .ReadFromJsonAsync<List<Event>>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            int.TryParse(
                HttpContext.Session.GetString("UserId"),
                out int organiserId);

            var ownEvents = events?
                .Where(e => e.OrganiserId == organiserId)
                .ToList() ?? new List<Event>();

            if (!ownEvents.Any(e => e.EventId == category.EventId))
            {
                return NotFound();
            }

            ViewBag.Events = new SelectList(
                ownEvents, "EventId", "EventName", category.EventId);

            var model = new EditCategoryViewModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                EventId = category.EventId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var eventClient = _httpClientFactory.CreateClient("RaceDayAPI");

                var events = await eventClient.GetFromJsonAsync<List<Event>>(
                    "api/Events/get_events");

                int.TryParse(
                    HttpContext.Session.GetString("UserId"),
                    out int organiserId);

                var ownEvents = events?
                    .Where(e => e.OrganiserId == organiserId)
                    .ToList() ?? new List<Event>();

                ViewBag.Events = new SelectList(
                    ownEvents, "EventId", "EventName", model.EventId);

                return View(model);
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var url = $"api/Categories/edit/{model.CategoryId}" +
                      $"?categoryName={Uri.EscapeDataString(model.CategoryName)}" +
                      $"&eventId={model.EventId}";

            var response = await client.PutAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Content(
                    $"Unable to update category.\nStatus: {response.StatusCode}\nError: {error}");
            }

            TempData["Success"] = "Category updated successfully.";

            return RedirectToAction("Index", new 
            {
                eventId = model.EventId 
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.DeleteAsync(
                $"api/Categories/delete/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                TempData["Error"] =
                    "This category cannot be deleted because it has related records.";
            }
            else if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    $"Unable to delete category: {response.StatusCode}";
            }
            else
            {
                TempData["Success"] = "Category deleted successfully.";
            }

            return RedirectToAction("Index", new
            {
                eventId
            });
        }
    }
}