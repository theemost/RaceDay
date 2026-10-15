using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Users : ControllerBase
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
    }
    [Route("api/[controller]")]
    [ApiController]
    public class EventTypes : ControllerBase
    {
    }

    [Route("api/[controller]")]
    [ApiController]
    public class Enrolments : ControllerBase
    {
    }

    [Route("api/[controller]")]
    [ApiController]
    public class Results : ControllerBase
    {
    }
}
