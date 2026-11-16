
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class ParticipantDashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ParticipantDashboardController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Participant")
            {
                return RedirectToAction("Index", "Events");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Retrieve the participant's own enrolments
            var enrolmentsResponse = await client.GetAsync(
                "api/Enrolments/my_enrolments");

            if (!enrolmentsResponse.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve your enrolments.");
            }

            var enrolments = await enrolmentsResponse.Content
                .ReadFromJsonAsync<List<Enrolment>>(options);

            // Retrieve the participant's own results
            var resultsResponse = await client.GetAsync(
                "api/Results/get_results");

            if (!resultsResponse.IsSuccessStatusCode)
            {
                return Content("Unable to retrieve your results.");
            }

            var results = await resultsResponse.Content
                .ReadFromJsonAsync<List<Result>>(options);

            var dashboard = new ParticipantDashboardViewModel
            {
                Enrolments = enrolments ?? new List<Enrolment>(),
                Results = results ?? new List<Result>()
            };

            return View("~/Views/Participant/Dashboard.cshtml", dashboard);
        }
    }
}
