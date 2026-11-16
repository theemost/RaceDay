using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;
using System.Security.Claims;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Results : ControllerBase
    {
        private readonly ResultsConnection db;

        public Results(ResultsConnection db)
        {
            this.db = db;
        }

        // Get results belonging to the logged-in user
        [Authorize(Roles = "Organiser,Participant")]
        [HttpGet("get_results")]
        public ActionResult<List<Result>> GetResults()
        {
            var userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            if (User.IsInRole("Organiser"))
            {
                return Ok(db.GetResultsForOrganiser(userId));
            }

            return Ok(db.GetResultsForParticipant(userId));
        }

        // Add a result
        // Add a result with automatic position calculation
        [Authorize(Roles = "Organiser")]
        [HttpPost("new_result")]
        public ActionResult<Result> AddResult(
            [FromQuery] int enrolmentId,
            [FromQuery] TimeSpan finishTime)
        {
            var organiserClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(organiserClaim, out int organiserId))
            {
                return Unauthorized();
            }

            if (finishTime < TimeSpan.Zero)
            {
                return BadRequest("Please provide a valid finishing time.");
            }

            // Check event ownership
            if (!db.OrganiserOwnsEnrolment(enrolmentId, organiserId))
            {
                return Forbid();
            }

            var result = db.AddResult(enrolmentId, finishTime);

            if (result == null)
            {
                return NotFound("Enrolment not found.");
            }

            return Ok(result);
        }

        // Get results for a specific event
        [Authorize(Roles = "Organiser")]
        [HttpGet("Events/{eventId}")]
        public ActionResult<List<Result>> GetResultsByEvent(int eventId)
        {
            var organiserClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(organiserClaim, out int organiserId))
            {
                return Unauthorized();
            }

            if (!db.OrganiserOwnsEvent(eventId, organiserId))
            {
                return Forbid();
            }

            return Ok(db.GetResultsByEventId(eventId));
        }

        // Recalculate positions for the organiser's events
        [Authorize(Roles = "Organiser")]
        [HttpPost("recalculate_positions")]
        public IActionResult RecalculatePositions()
        {
            var organiserClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(organiserClaim, out int organiserId))
            {
                return Unauthorized();
            }

            try
            {
                db.RecalculatePositionsForOrganiser(organiserId);

                return Ok(new
                {
                    message = "Result positions recalculated successfully."
                });
            }
            catch (InvalidOperationException error)
            {
                return StatusCode(500, new
                {
                    message = error.Message
                });
            }
        }
    }
}