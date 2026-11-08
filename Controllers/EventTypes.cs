using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    /*[Route("api/[controller]")]
    [ApiController]
    public class EventTypes : ControllerBase
    {  
        private readonly Db_connection db;

        public EventTypes(Db_connection db)
        {
            this.db = db;
        }

        [HttpGet]
        public List<EventType> GetEventTypes()
        {
            Db_connection db = new Db_connection();
            List<EventType> data_found = db.GetEventTypes();
            return data_found;
        }

        [HttpGet("{eventTypeId}")]
        public EventType? GetEventType(int eventTypeId)
        {
            Db_connection db = new Db_connection();
            EventType? data_found = db.GetEventType(eventTypeId);
            return data_found;
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public EventType? AddEventType(string eventTypeName)
        {
            Db_connection db = new Db_connection();
            EventType? data_found = db.AddEventType(eventTypeName);
            return data_found;
        }
    }*/
}
