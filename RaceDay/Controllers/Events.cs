using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Events : ControllerBase
    {
        private readonly EventsConnection db;

        public Events(EventsConnection db)
        {
            this.db = db;
        }

        [HttpGet("get_events")]
        public List<Event> GetEvents()
        {
            return db.GetEvents();
        }

        [HttpGet("get_event/{eventId}")]
        public Event? GetEvent(int eventId)
        {
            return db.GetEvent(eventId);
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public ActionResult<Event> AddEvent([FromBody] CreateEventRequest request)
        {
            var organiserClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier);

            if (organiserClaim == null ||
                !int.TryParse(organiserClaim.Value, out int organiserId))
            {
                return Unauthorized();
            }

            Event? data = db.AddEvent(
                request.EventName,
                request.Description,
                request.EventDate,
                request.Location,
                request.EventType,
                organiserId);

            if (data == null)
            {
                return BadRequest();
            }

            return Ok(data);
        }


        [Authorize(Roles = "Organiser")]
        [HttpPut("{eventId}")]
        public ActionResult<Event> EditEvent(
            int eventId,
            [FromBody] EditEventRequest request)
        {
            var organiserClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (organiserClaim == null ||
                !int.TryParse(organiserClaim.Value, out int organiserId))
            {
                return Unauthorized();
            }

            Event? data = db.EditEvent(
                eventId,
                request.EventName,
                request.Description,
                request.EventDate,
                request.Location,
                request.EventTypeId,
                organiserId);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [Authorize(Roles = "Organiser")]
        [HttpDelete("{eventId}")]
        public IActionResult DeleteEvent(int eventId)
        {
            var organiserClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (organiserClaim == null ||
                !int.TryParse(organiserClaim.Value, out int organiserId))
            {
                return Unauthorized();
            }

            try
            {
                bool deleted = db.DeleteEvent(eventId, organiserId);

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
    }
}

