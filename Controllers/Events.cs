using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication : ControllerBase
    {
        [HttpGet("login/")]
        public string login(string username, string password)
        {
            Db_connection db = new Db_connection();
            string message = db.login(username, password);
            return message;
        }
        
        [HttpPost("register/")]
        public string register(string firstName, string lastName, string emailAddress, string username, string password, string role)
        {
            Db_connection db = new Db_connection();
            string message = db.register(firstName, lastName, emailAddress, username, password, role);
            return message;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class Events : ControllerBase
    {
        [HttpGet]
        public List<Event> GetEvents()
        {
            Db_connection db = new Db_connection();
            List<Event> events = db.GetEvents();
            return events;
        }

        [HttpGet("{eventId}")]
        public Event? GetEvent(int eventId)
        {
            Db_connection db = new Db_connection();
            Event? data_found = db.GetEvent(eventId);
            return data_found;
        }

        [HttpPost]
        public Event? AddEvent(string eventName, string description, DateTime eventDate, string location, int eventType, int organiser)
        {
            Db_connection db = new Db_connection();
            Event? data_found = db.AddEvent(eventName, description, eventDate, location, eventType, organiser);
            return data_found;
        }

        [HttpPut("{eventId}")]
        public Event? EditEvent(int eventId, string eventName, string description, DateTime eventDate, string location, int eventTypeId, int organiserId)
        {
            Db_connection db = new Db_connection();
            Event? data_found = db.EditEvent(eventId, eventName, description, eventDate, location, eventTypeId, organiserId);
            return data_found;
        }

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
        }
    }

    //Event Types Controller
    [Route("api/[controller]")]
    [ApiController]
    public class EventTypes : ControllerBase
    {
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
        
        [HttpPost]
        public EventType? AddEventType(string eventTypeName)
        {
            Db_connection db = new Db_connection();
            EventType? data_found = db.AddEventType(eventTypeName);
            return data_found;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class Categories : ControllerBase
    {
        [HttpPost]
        public Category? AddCategory(string categoryName, int eventId)
        {
            Db_connection db = new Db_connection();
            Category? data_found = db.AddCategory(categoryName, eventId);
            return data_found;
        }

        [HttpGet]
        public List<Category> GetCategories()
        {
            Db_connection db = new Db_connection();
            List<Category> data_found = db.GetCategories();
            return data_found;
        }

        [HttpGet("{categoryId}")]
        public Category? GetCategory(int categoryId)
        {
            Db_connection db = new Db_connection();
            Category? data_found = db.GetCategory(categoryId);
            return data_found;
        }
        
        [HttpPut("{categoryId}")]
        public Category? EditCategory(int categoryId, string categoryName, int eventId)
        {
            Db_connection db = new Db_connection();
            Category? data_found = db.EditCategory(categoryId, categoryName, eventId);
            return data_found;
        }

        [HttpDelete("{categoryId}")]
        public IActionResult DeleteCategory(int categoryId)
        {
            Db_connection db = new Db_connection();

            bool deleted = db.DeleteCategory(categoryId);

            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class Enrolments : ControllerBase
    {
        [HttpGet]
        public List<Enrolment> GetEnrolments()
        {
            Db_connection db = new Db_connection();
            List<Enrolment> data_found = db.GetEnrolments();
            return data_found;
        }

        [HttpGet("get_enrolment/{enrolmentId}")]
        public Enrolment? GetEnrolment(int enrolmentId)
        {
            Db_connection db = new Db_connection();
            Enrolment? data_found = db.GetEnrolment(enrolmentId);
            return data_found;
        }

        [HttpPost("new_enrolment")]
        public Enrolment? AddEnrolment(int participantId, int eventId, int categoryId, string enrolmentStatus)
        {
            Db_connection db = new Db_connection();
            Enrolment? data_found = db.AddEnrolment(participantId, eventId, categoryId, enrolmentStatus);
            return data_found;
        }

        [HttpPut("edit_enrolment/{enrolmentId}")]
        public Enrolment? EditEnrolment(int enrolmentId, string participantId, int eventId, int categoryId, string enrolmentStatus)
        {
            Db_connection db = new Db_connection();
            Enrolment? data_found = db.EditEnrolment(enrolmentId, participantId, categoryId, eventId, enrolmentStatus);
            return data_found;
        }

        [HttpDelete("delete_enrolment/{enrolmentId}")]
        public IActionResult DeleteEnrolment(int enrolmentId)
        {
            Db_connection db = new Db_connection();

            bool deleted = db.DeleteEnrolment(enrolmentId);

            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class Results : ControllerBase
    {
    }
}
