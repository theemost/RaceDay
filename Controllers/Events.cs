using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Events : ControllerBase
    {
        [HttpGet]
        public string GetEvents()
        {
            Db_connection db = new Db_connection();
            string message = db.GetEvents();
            return message;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class Users : ControllerBase
    {
        [HttpGet]
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
}
