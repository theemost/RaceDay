using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;
using System.Security.Claims;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Enrolments : ControllerBase
    {
        private readonly EnrolmentsConnection db;

        public Enrolments(EnrolmentsConnection db)
        {
            this.db = db;
        }

        // Get all enrolments
        [Authorize(Roles = "Organiser")]
        [HttpGet("enrolments")]
        public ActionResult<List<Enrolment>> GetEnrolments()
        {
            var organiserClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(organiserClaim, out int organiserId))
            {
                return Unauthorized();
            }

            var enrolments = db.GetEnrolmentsForOrganiser(organiserId);

            return Ok(enrolments);
        }

        // Get enrolments belonging to the logged-in participant
        [Authorize(Roles = "Participant")]
        [HttpGet("my_enrolments")]
        public ActionResult<List<Enrolment>> GetMyEnrolments()
        {
            var participantClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(participantClaim, out int participantId))
            {
                return Unauthorized();
            }

            var enrolments = db.GetEnrolments()
                .Where(e => e.ParticipantId == participantId)
                .ToList();

            return Ok(enrolments);
        }

        // Get one enrolment
        [Authorize(Roles = "Organiser,Participant")]
        [HttpGet("get_enrolment/{enrolmentId}")]
        public ActionResult<Enrolment> GetEnrolment(int enrolmentId)
        {
            var data = db.GetEnrolment(enrolmentId);

            if (data == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            // Organisers can only view enrolments for their own events
            if (User.IsInRole("Organiser"))
            {
                if (!db.OrganiserOwnsEnrolment(enrolmentId, userId))
                {
                    return Forbid();
                }
            }
            // Participants can only view their own enrolments
            else if (User.IsInRole("Participant"))
            {
                if (data.ParticipantId != userId)
                {
                    return Forbid();
                }
            }

            return Ok(data);
        }

        // Add a new enrolment
        [Authorize(Roles = "Participant")]
        [HttpPost("new_enrolment")]
        public ActionResult<Enrolment> AddEnrolment(
            [FromQuery] int eventId,
            [FromQuery] int categoryId)
        {
            var participantClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(participantClaim, out int participantId))
            {
                return Unauthorized();
            }

            try
            {
                var data = db.AddEnrolment(
                    participantId,
                    eventId,
                    categoryId,
                    "Pending");

                if (data == null)
                {
                    return BadRequest(
                        "Invalid event or category. Make sure the category belongs to the selected event.");
                }

                return CreatedAtAction(
                    nameof(GetEnrolment),
                    new { enrolmentId = data.EnrolmentId },
                    data);
            }
            catch (InvalidOperationException error)
            {
                return Conflict(new { message = error.Message });
            }
        }

        // Edit an enrolment
        [Authorize(Roles = "Organiser")]
        [HttpPut("edit_enrolment/{enrolmentId}")]
        public ActionResult<Enrolment> EditEnrolment(
    int enrolmentId,
    [FromQuery] int categoryId,
    [FromQuery] int eventId,
    [FromQuery] string enrolmentStatus)
        {
            try
            {
                var organiserClaim = User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                if (!int.TryParse(organiserClaim, out int organiserId))
                {
                    return Unauthorized();
                }

                // Check whether the enrolment belongs to this organiser
                if (!db.OrganiserOwnsEnrolment(enrolmentId, organiserId))
                {
                    return Forbid();
                }

                var existingEnrolment = db.GetEnrolment(enrolmentId);

                if (existingEnrolment == null)
                {
                    return NotFound("Enrolment not found.");
                }

                if (!db.OrganiserOwnsEvent(eventId, organiserId))
                {
                    return Forbid();
                }

                var data = db.EditEnrolment(
                    enrolmentId,
                    existingEnrolment.ParticipantId,
                    categoryId,
                    eventId,
                    enrolmentStatus);

                if (data == null)
                {
                    return BadRequest("Invalid category or event selection.");
                }

                return Ok(data);
            }
            catch (InvalidOperationException error)
            {
                return Conflict(new { message = error.Message });
            }
        }

        // Delete an enrolment
        [Authorize(Roles = "Organiser")]
        [HttpDelete("delete_enrolment/{enrolmentId}")]
        public IActionResult DeleteEnrolment(int enrolmentId)
        {
            try
            {
                var organiserClaim = User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                if (!int.TryParse(organiserClaim, out int organiserId))
                {
                    return Unauthorized();
                }

                // Check whether the enrolment exists
                var existingEnrolment = db.GetEnrolment(enrolmentId);

                if (existingEnrolment == null)
                {
                    return NotFound("Enrolment not found.");
                }

                // Check whether this organiser owns the event
                if (!db.OrganiserOwnsEnrolment(enrolmentId, organiserId))
                {
                    return Forbid();
                }

                bool deleted = db.DeleteEnrolment(enrolmentId);

                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException error)
            {
                return Conflict(new { message = error.Message });
            }
        }

        // Approve or reject an enrolment
        [Authorize(Roles = "Organiser")]
        [HttpPut("update_status/{enrolmentId}")]
        public ActionResult<Enrolment> UpdateEnrolmentStatus(
            int enrolmentId,
            [FromQuery] string enrolmentStatus)
        {
            var organiserClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(organiserClaim, out int organiserId))
            {
                return Unauthorized();
            }

            // Only allow these status changes
            if (enrolmentStatus != "Approved" &&
                enrolmentStatus != "Rejected")
            {
                return BadRequest("Invalid enrolment status.");
            }

            // Check event ownership
            if (!db.OrganiserOwnsEnrolment(enrolmentId, organiserId))
            {
                return Forbid();
            }

            var existingEnrolment = db.GetEnrolment(enrolmentId);

            if (existingEnrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            try
            {
                var updatedEnrolment = db.EditEnrolment(
                    enrolmentId,
                    existingEnrolment.ParticipantId,
                    existingEnrolment.CategoryId,
                    existingEnrolment.EventId,
                    enrolmentStatus);

                if (updatedEnrolment == null)
                {
                    return NotFound("Unable to update enrolment.");
                }

                return Ok(updatedEnrolment);
            }
            catch (InvalidOperationException error)
            {
                return Conflict(new { message = error.Message });
            }
        }
    }
}