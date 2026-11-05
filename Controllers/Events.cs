using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaceDay.Controllers
{
//    [Authorize(Role = "Organiser, Participant")]
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

        //[Authorize(Roles = "Organiser")]
        [HttpPost]
        public ActionResult<Event> AddEvent(string eventName, string description, DateTime eventDate, string location, int eventType, int organiser)
        {

            Event? data = db.AddEvent(
                    eventName,
                    description,
                    eventDate,
                    location,
                    eventType,
                    organiser);

            if (data == null)
            {
                return BadRequest();
            }

            return data;
        }


        /*[Authorize(Roles = "Organiser")]
        [HttpPut("{eventId}")]
        public Event? EditEvent(int eventId, string eventName, string description, DateTime eventDate, string location, int eventTypeId, int organiserId)
        {
            Db_connection db = new Db_connection();
            Event? data_found = db.EditEvent(eventId, eventName, description, eventDate, location, eventTypeId, organiserId);
            return data_found;
        }

        [Authorize(Roles = "Organiser")]
        [HttpDelete("{eventId}")]
        public IActionResult DeleteEvent(int eventId)
        {
            Db_connection db = new Db_connection();

            bool deleted = db.DeleteEvent(eventId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }*/
    }
}

