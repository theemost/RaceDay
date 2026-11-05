using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;
using System.Security.Claims;

namespace RaceDay.Controllers
{
    /*[Route("api/[controller]")]
    [ApiController]
    public class UserProfile : ControllerBase
    {
        private readonly Db_connection db;

        public UserProfile(Db_connection db)
        {
            this.db = db;
        }

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
    }*/
}
