using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using System.Security.Claims;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfile : ControllerBase
    {
        private readonly UserProfileConnection db;

        public UserProfile(UserProfileConnection db)
        {
            this.db = db;
        }

        // Get logged-in user's profile
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int id))
            {
                return Unauthorized();
            }

            var user = db.GetUser(id);

            if (user == null)
            {
                return NotFound("User profile not found.");
            }

            return Ok(ProfileResponse(user));
        }

        // Update logged-in user's profile
        [Authorize]
        [HttpPut("update_profile")]
        public IActionResult UpdateProfile(
            [FromQuery] string firstName,
            [FromQuery] string lastName,
            [FromQuery] string emailAddress,
            [FromQuery] string username)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int id))
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(emailAddress) ||
                string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("All profile fields are required.");
            }

            try
            {
                var updatedUser = db.UpdateUser(
                    id,
                    firstName,
                    lastName,
                    emailAddress,
                    username);

                if (updatedUser == null)
                {
                    return NotFound("User profile not found.");
                }

                return Ok(ProfileResponse(updatedUser));
            }
            catch (InvalidOperationException error)
            {
                return Conflict(new { message = error.Message });
            }
        }

        // Change password
        [Authorize]
        [HttpPut("change_password")]
        public IActionResult ChangePassword(
            [FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int id))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check that the new password is different
            if (request.CurrentPassword == request.NewPassword)
            {
                return BadRequest(new
                {
                    message = "Your new password must be different from your current password."
                });
            }

            bool changed = db.ChangePassword(
                id,
                request.CurrentPassword,
                request.NewPassword);

            if (!changed)
            {
                return BadRequest(new
                {
                    message = "Your current password is incorrect."
                });
            }

            return Ok(new
            {
                message = "Password changed successfully."
            });
        }

        private static object ProfileResponse(User user)
        {
            return new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.EmailAddress,
                Username = user.username,
                user.Role
            };
        }


    }
}