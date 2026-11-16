using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication : ControllerBase
    {
        private readonly Authenticator authenticate;

        public Authentication(Authenticator authenticate)
        {
            this.authenticate = authenticate;
        }

        [HttpGet("login/")]
        public IActionResult login(string username, string password)
        {
            string result = authenticate.login(username, password);

            if (result.Split('.').Length != 3)
            {
                return Unauthorized(new { message = result });
            }

            return Ok(new { token = result });
        }

        [HttpPost("register/")]
        public IActionResult register([FromBody] RegisterRequest request)
        {
            string message = authenticate.register(
                request.FirstName,
                request.LastName,
                request.EmailAddress,
                request.Username,
                request.Password,
                request.Role
            );

            if (message != "User registered successfully")
            {
                return Conflict(new { message });
            }

            return Ok(new { message });
        }
    }
}