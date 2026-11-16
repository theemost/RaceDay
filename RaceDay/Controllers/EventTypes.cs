using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventTypes : ControllerBase
    {
        private readonly EventTypesConnection db;

        public EventTypes(EventTypesConnection db)
        {
            this.db = db;
        }

        [HttpGet]
        public List<EventType> GetEventTypes()
        {
            return db.GetEventTypes();
        }

        [HttpGet("{eventTypeId}")]
        public EventType? GetEventType(int eventTypeId)
        {
            return db.GetEventType(eventTypeId);
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public EventType? AddEventType(string eventTypeName)
        {
            return db.AddEventType(eventTypeName);
        }
    }
}