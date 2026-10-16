using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication : ControllerBase
    {

        private readonly IConfiguration configuration;

        public Authentication(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("login/")]
        public string login(string username, string password)
        {
            Authenticator authenticate = new Authenticator(configuration);
            string message = authenticate.login(username, password);
            return message;
        }

        [HttpPost("register/")]
        public string register(string firstName, string lastName, string emailAddress, string username, string password, string role)
        {
            Authenticator authenticate = new Authenticator(configuration);
            string message = authenticate.register(firstName, lastName, emailAddress, username, password, role);
            return message;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UserInformation : ControllerBase
    {
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            string? userId = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userId, out int id))
            {
                return Unauthorized();
            }

            Db_connection db = new Db_connection();

            User? user = db.GetUser(id);

            if (user == null)
            {
                return NotFound("User profile not found.");
            }

            return Ok(user);
        }

        [Authorize]
        [HttpPut("update_profile")]
        public IActionResult UpdateProfile(string firstName, string lastName, string emailAddress, string username)
        {
            string? userId = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            if (!int.TryParse(userId, out int id))
            {
                return Unauthorized();
            }
            Db_connection db = new Db_connection();
            User? updatedUser = db.UpdateUser(id, firstName, lastName, emailAddress, username);
            if (updatedUser == null)
            {
                return NotFound("User profile not found.");
            }
            return Ok(updatedUser);
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

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public Event? AddEvent(string eventName, string description, DateTime eventDate, string location, int eventType, int organiser)
        {
            Db_connection db = new Db_connection();
            Event? data_found = db.AddEvent(eventName, description, eventDate, location, eventType, organiser);
            return data_found;
        }

        [Authorize(Roles = "Organiser")]
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

        [Authorize(Roles = "Organiser")]
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
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public Category? AddCategory(string categoryName, int eventId)
        {
            Db_connection db = new Db_connection();
            Category? data_found = db.AddCategory(categoryName, eventId);
            return data_found;
        }

        [HttpGet("get_categories")]
        public List<Category> GetCategories()
        {
            Db_connection db = new Db_connection();
            List<Category> data_found = db.GetCategories();
            return data_found;
        }

        [HttpGet("get_category/{categoryId}")]
        public Category? GetCategory(int categoryId)
        {
            Db_connection db = new Db_connection();
            Category? data_found = db.GetCategory(categoryId);
            return data_found;
        }

        [Authorize(Roles = "Organiser")]
        [HttpPut("edit/{categoryId}")]
        public Category? EditCategory(int categoryId, string categoryName, int eventId)
        {
            Db_connection db = new Db_connection();
            Category? data_found = db.EditCategory(categoryId, categoryName, eventId);
            return data_found;
        }

        [Authorize(Roles = "Organiser")]
        [HttpDelete("delete/{categoryId}")]
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
        [Authorize(Roles = "Organiser")]
        [HttpGet("enrolments")]
        public List<Enrolment> GetEnrolments()
        {
            Db_connection db = new Db_connection();
            List<Enrolment> data_found = db.GetEnrolments();
            return data_found;
        }

        [Authorize]
        [HttpGet("get_enrolment/{enrolmentId}")]
        public Enrolment? GetEnrolment(int enrolmentId)
        {
            Db_connection db = new Db_connection();
            Enrolment? data_found = db.GetEnrolment(enrolmentId);
            return data_found;
        }

        [Authorize]
        [HttpPost("new_enrolment")]
        public Enrolment? AddEnrolment(int participantId, int eventId, int categoryId, string enrolmentStatus)
        {
            Db_connection db = new Db_connection();
            Enrolment? data_found = db.AddEnrolment(participantId, eventId, categoryId, enrolmentStatus);
            return data_found;
        }

        [Authorize(Roles = "Organiser")]
        [HttpPut("edit_enrolment/{enrolmentId}")]
        public Enrolment? EditEnrolment(int enrolmentId, int participantId, int categoryId, int eventId, string enrolmentStatus)
        {
            Db_connection db = new Db_connection();
            Enrolment? data_found = db.EditEnrolment(enrolmentId, participantId, categoryId, eventId, enrolmentStatus);
            return data_found;
        }

        [Authorize(Roles = "Organiser")]
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
        [Authorize]
        [HttpGet("get_results")]
        public List<Result> GetResults()
        {
            Console.WriteLine("Results controller called.");
            Db_connection db = new Db_connection();
            List<Result> result_data = db.GetResults();
            return result_data;
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost("new_result")]
        public Result? AddResult(int enrolmentId, TimeSpan finishTime, int position)
        {
            Db_connection db = new Db_connection();
            Result? data_found = db.AddResult(enrolmentId, finishTime, position);
            return data_found;
        }

        [Authorize(Roles = "Organiser")]
        [HttpGet("Events/{eventId}")]
        public List<Result> GetResults(int eventId)
        {
            Db_connection db = new Db_connection();
            List<Result> data_found = db.GetResultsByEventId(eventId);
            return data_found;
        }
    } 
}
