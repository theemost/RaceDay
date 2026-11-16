using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Globalization;

namespace RaceDay.MVC.Controllers
{
    public class ResultsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ResultsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // View Results
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

            var response = await client.GetAsync(
                "api/Results/get_results");

            if (!response.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve results.");
            }

            var results = await response.Content
                .ReadFromJsonAsync<List<Result>>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(results ?? new List<Result>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Organiser")
            {
                return RedirectToAction("Index", "Events");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Retrieve enrolments belonging to this organiser's events
            var response = await client.GetAsync(
                "api/Enrolments/enrolments");

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

            ViewBag.Enrolments = enrolments ?? new List<Enrolment>();

            return View(new CreateResultViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateResultViewModel model)
        {
            // Only Organisers can record results
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Index", "Events");
            }

            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // Validate finishing time
            if (!TimeSpan.TryParse(
                model.FinishTime,
                CultureInfo.InvariantCulture,
                out TimeSpan finishTime) ||
                finishTime < TimeSpan.Zero)
            {
                ModelState.AddModelError(
                    "FinishTime",
                    "Please enter a valid finishing time, for example 00:25:30.");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Reload enrolments if validation fails
            if (!ModelState.IsValid)
            {
                var enrolmentsResponse = await client.GetAsync(
                    "api/Enrolments/enrolments");

                if (enrolmentsResponse.IsSuccessStatusCode)
                {
                    ViewBag.Enrolments = await enrolmentsResponse.Content
                        .ReadFromJsonAsync<List<Enrolment>>(
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }) ?? new List<Enrolment>();
                }
                else
                {
                    ViewBag.Enrolments = new List<Enrolment>();
                }

                return View(model);
            }

            // Submit the result to the API
            var url = "api/Results/new_result" +
                      $"?enrolmentId={model.EnrolmentId}" +
                      $"&finishTime={Uri.EscapeDataString(finishTime.ToString("c", CultureInfo.InvariantCulture))}";

            var response = await client.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to record the result. Please check the enrolment and try again.");

                var enrolmentsResponse = await client.GetAsync(
                    "api/Enrolments/enrolments");

                if (enrolmentsResponse.IsSuccessStatusCode)
                {
                    ViewBag.Enrolments = await enrolmentsResponse.Content
                        .ReadFromJsonAsync<List<Enrolment>>(
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }) ?? new List<Enrolment>();
                }
                else
                {
                    ViewBag.Enrolments = new List<Enrolment>();
                }

                return View(model);
            }

            TempData["Success"] = "Result recorded successfully.";

            return RedirectToAction("Index");
        }
    }
}