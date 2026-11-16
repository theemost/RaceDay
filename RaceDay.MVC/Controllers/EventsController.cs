using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EventsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var response = await client.GetAsync("api/Events/get_events");

            if (!response.IsSuccessStatusCode)
            {
                return Content($"API Error: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            var events = JsonSerializer.Deserialize<List<Event>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View(events ?? new List<Event>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // Retrieve event details
            var response = await client.GetAsync(
                $"api/Events/get_event/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var eventDetails = await response.Content
                .ReadFromJsonAsync<Event>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (eventDetails == null)
            {
                return NotFound();
            }

            // Retrieve categories belonging to this event
            var categoriesResponse = await client.GetAsync(
                $"api/Categories/event/{id}");

            var categories = new List<Category>();

            if (categoriesResponse.IsSuccessStatusCode)
            {
                categories = await categoriesResponse.Content
                    .ReadFromJsonAsync<List<Category>>(
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? new List<Category>();
            }

            ViewBag.Categories = categories;

            // Check whether the logged-in organiser owns this event
            int.TryParse(
                HttpContext.Session.GetString("UserId"),
                out int userId);

            bool isEventOrganiser =
                HttpContext.Session.GetString("Role") == "Organiser"
                && eventDetails.OrganiserId == userId;

            ViewBag.IsEventOrganiser = isEventOrganiser;

            // Retrieve enrolments only for the event's organiser
            var enrolments = new List<Enrolment>();

            if (isEventOrganiser)
            {
                var token = HttpContext.Session.GetString("JWToken");

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                    var enrolmentsResponse = await client.GetAsync(
                        "api/Enrolments/enrolments");

                    if (enrolmentsResponse.IsSuccessStatusCode)
                    {
                        var allEnrolments = await enrolmentsResponse.Content
                            .ReadFromJsonAsync<List<Enrolment>>(
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                        enrolments = allEnrolments?
                            .Where(e => e.EventId == id)
                            .ToList() ?? new List<Enrolment>();
                    }
                    else
                    {
                        ViewBag.EnrolmentsError =
                            "Unable to retrieve event enrolments.";
                    }
                }
            }

            ViewBag.Enrolments = enrolments;

            return View(eventDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEnrolmentStatus(
    int enrolmentId,
    int eventId,
    string status)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Index", "Events");
            }

            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (status != "Approved" && status != "Rejected")
            {
                TempData["Error"] = "Invalid enrolment status.";
                return RedirectToAction("Details", new { id = eventId });
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var url = $"api/Enrolments/update_status/{enrolmentId}" +
                      $"?enrolmentStatus={Uri.EscapeDataString(status)}";

            var response = await client.PutAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    $"Unable to update enrolment status: {response.StatusCode}";
            }
            else
            {
                TempData["Success"] =
                    $"Enrolment successfully {status.ToLower()}.";
            }

            return RedirectToAction("Details", new { id = eventId });
        }

        //Event Creation Methods
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var response = await client.GetAsync("api/EventTypes");

            if (!response.IsSuccessStatusCode)
            {
                return Content($"Unable to load event types: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            var eventTypes = JsonSerializer.Deserialize<List<EventType>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            ViewBag.EventTypes = eventTypes ?? new List<EventType>();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return await Create();
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var token = HttpContext.Session.GetString("JWToken");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var eventData = new
            {
                eventName = model.EventName,
                description = model.Description,
                eventDate = model.EventDate,
                location = model.Location,
                eventType = model.EventTypeId
            };

            var response = await client.PostAsJsonAsync(
                "api/Events", eventData);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty,
                    $"Unable to create event: {response.StatusCode}");

                return await Create();
            }
            var createdEvent = await response.Content
                .ReadFromJsonAsync<Event>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (createdEvent == null || createdEvent.EventId <= 0)
            {
                return Content(
                    "Event was created, but the API did not return its event ID.");
            }

            return RedirectToAction(
                "Index",
                "Categories",
                new { eventId = createdEvent.EventId });
        }

        //Edit event method
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var response = await client.GetAsync(
                $"api/Events/get_event/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();

            var eventDetails = JsonSerializer.Deserialize<Event>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (eventDetails == null)
            {
                return NotFound();
            }

            var model = new EditEventViewModel
            {
                EventId = eventDetails.EventId,
                EventName = eventDetails.EventName,
                Description = eventDetails.Description,
                EventDate = eventDetails.EventDate,
                Location = eventDetails.Location,
                EventTypeId = eventDetails.EventTypeId
            };

            var eventTypesResponse = await client.GetAsync("api/EventTypes");

            if (!eventTypesResponse.IsSuccessStatusCode)
            {
                return Content("Unable to load event types.");
            }

            var eventTypesJson = await eventTypesResponse.Content.ReadAsStringAsync();

            ViewBag.EventTypes = JsonSerializer.Deserialize<List<EventType>>(
                eventTypesJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<EventType>();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return await Edit(model.EventId);
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var token = HttpContext.Session.GetString("JWToken");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var eventData = new
            {
                eventName = model.EventName,
                description = model.Description,
                eventDate = model.EventDate,
                location = model.Location,
                eventTypeId = model.EventTypeId
            };

            var response = await client.PutAsJsonAsync(
                $"api/Events/{model.EventId}", eventData);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty,
                    $"Unable to update event: {response.StatusCode}");

                return await Edit(model.EventId);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.DeleteAsync($"api/Events/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                TempData["Error"] =
                    "This event cannot be deleted because it has related records.";
            }
            else if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                return Content(
                    $"Delete failed!\nStatus: {response.StatusCode}\nError: {errorMessage}");
            }
            else
            {
                TempData["Success"] = "Event deleted successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}