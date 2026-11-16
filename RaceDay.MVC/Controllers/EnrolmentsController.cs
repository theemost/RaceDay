using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EnrolmentsController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Participant" && role != "Organiser")
            {
                return RedirectToAction("Index", "Events");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            string endpoint = role == "Participant"
                ? "api/Enrolments/my_enrolments"
                : "api/Enrolments/enrolments";

            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve enrolments.");
            }

            var enrolments = await response.Content
                .ReadFromJsonAsync<List<Enrolment>>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(enrolments ?? new List<Enrolment>());
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? eventId)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Participant")
            {
                return RedirectToAction("Index", "Events");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Retrieve available events
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

            // Retrieve available categories
            var categoriesResponse = await client.GetAsync(
                "api/Categories/get_categories");

            if (!categoriesResponse.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve categories.");
            }

            var categories = await categoriesResponse.Content
                .ReadFromJsonAsync<List<Category>>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            ViewBag.Events = events ?? new List<Event>();
            ViewBag.Categories = categories ?? new List<Category>();

            return View(new CreateEnrolmentViewModel
            {
                EventId = eventId ?? 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEnrolmentViewModel model)
        {
            // Only Participants can enrol
            if (HttpContext.Session.GetString("Role") != "Participant")
            {
                return RedirectToAction("Index", "Events");
            }

            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Check model validation
            if (!ModelState.IsValid)
            {
                await LoadEnrolmentOptions(client);
                return View(model);
            }

            // Send the enrolment to the API
            var url = $"api/Enrolments/new_enrolment" +
                      $"?eventId={model.EventId}" +
                      $"&categoryId={model.CategoryId}";

            var response = await client.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to submit enrolment. Please check your event and category selection.");

                await LoadEnrolmentOptions(client);

                return View(model);
            }

            TempData["Success"] =
                "Your enrolment was submitted successfully and is awaiting approval.";

            return RedirectToAction("Index");
        }

        private async Task LoadEnrolmentOptions(HttpClient client)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var eventsResponse = await client.GetAsync(
                "api/Events/get_events");

            var categoriesResponse = await client.GetAsync(
                "api/Categories/get_categories");

            var events = eventsResponse.IsSuccessStatusCode
                ? await eventsResponse.Content.ReadFromJsonAsync<List<Event>>(options)
                : new List<Event>();

            var categories = categoriesResponse.IsSuccessStatusCode
                ? await categoriesResponse.Content.ReadFromJsonAsync<List<Category>>(options)
                : new List<Category>();

            ViewBag.Events = events ?? new List<Event>();
            ViewBag.Categories = categories ?? new List<Category>();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
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

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.GetAsync(
                $"api/Enrolments/get_enrolment/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound("Enrolment not found.");
            }

            var enrolment = await response.Content
                .ReadFromJsonAsync<Enrolment>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (enrolment == null)
            {
                return NotFound();
            }

            var model = new EditEnrolmentViewModel
            {
                EnrolmentId = enrolment.EnrolmentId,
                EventId = enrolment.EventId,
                CategoryId = enrolment.CategoryId,
                EnrolmentStatus = enrolment.EnrolmentStatus
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditEnrolmentViewModel model)
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

            if (string.IsNullOrEmpty(model.EnrolmentStatus) ||
                !new[] { "Pending", "Approved", "Rejected" }
                    .Contains(model.EnrolmentStatus))
            {
                ModelState.AddModelError(
                    "EnrolmentStatus", "Please select a valid enrolment status.");

                return View(model);
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var url = $"api/Enrolments/edit_enrolment/{model.EnrolmentId}" +
                      $"?categoryId={model.CategoryId}" +
                      $"&eventId={model.EventId}" +
                      $"&enrolmentStatus={Uri.EscapeDataString(model.EnrolmentStatus)}";

            var response = await client.PutAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to update the enrolment. Please try again.");

                return View(model);
            }

            TempData["Success"] = "Enrolment status updated successfully.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
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

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.DeleteAsync(
                $"api/Enrolments/delete_enrolment/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                TempData["Error"] =
                    "This enrolment cannot be deleted because it has related results.";
            }
            else if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Unable to delete the enrolment.";
            }
            else
            {
                TempData["Success"] = "Enrolment deleted successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}