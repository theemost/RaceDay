using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly Authenticator db;

        public Authentication(IConfiguration configuration, Db_connection db)
        {
            this.db = db;
            this.configuration = configuration;
        }

        [HttpGet("login/")]
        public IActionResult login(string username, string password)
        {
            Authenticator authenticate = new Authenticator(configuration);
            string result = authenticate.login(username, password);

            // A JWT has three parts separated by two dots.
            if (result.Split('.').Length != 3)
            {
                return Unauthorized(new { message = result });
            }

            return Ok(new { token = result });
        }

        [HttpPost("register/")]
        public string register(string firstName, string lastName, string emailAddress, string username, string password, string role)
        {
            Authenticator authenticate = new Authenticator(configuration);
            string message = authenticate.register(firstName, lastName, emailAddress, username, password, role);
            return message;
        }           
    }
}
