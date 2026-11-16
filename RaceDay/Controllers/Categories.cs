using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using System.Security.Claims;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Categories : ControllerBase
    {
        private readonly CategoriesConnection db;

        public Categories(CategoriesConnection db)
        {
            this.db = db;
        }

        [HttpGet("get_categories")]
        public List<Category> GetCategories()
        {
            return db.GetCategories();
        }

        [HttpGet("event/{eventId}")]
        public ActionResult<List<Category>> GetCategoriesByEvent(int eventId)
        {
            var categories = db.GetCategoriesByEventId(eventId);

            return Ok(categories);
        }

        [HttpGet("get_category/{categoryId}")]
        public Category? GetCategory(int categoryId)
        {
            return db.GetCategory(categoryId);
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public ActionResult<Category> AddCategory([FromQuery] string categoryName, [FromQuery] int eventId)
        {
            var organiserClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (organiserClaim == null ||
                !int.TryParse(organiserClaim.Value, out int organiserId))
            {
                return Unauthorized();
            }

            try
            {
                Category? data = db.AddCategory(
                    categoryName, eventId, organiserId);

                if (data == null)
                {
                    return NotFound("Event not found or you do not own this event.");
                }

                return Ok(data);
            }
            catch (InvalidOperationException error)
            {
                return Conflict(new { message = error.Message });
            }
        }

        [Authorize(Roles = "Organiser")]
        [HttpPut("edit/{categoryId}")]
        public ActionResult<Category> EditCategory(
    int categoryId,
    [FromQuery] string categoryName,
    [FromQuery] int eventId)
        {
            var organiserClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (organiserClaim == null ||
                !int.TryParse(organiserClaim.Value, out int organiserId))
            {
                return Unauthorized();
            }

            try
            {
                Category? data = db.EditCategory(
                    categoryId, categoryName, eventId, organiserId);

                if (data == null)
                {
                    return NotFound("Category or event not found, or you do not own the event.");
                }

                return Ok(data);
            }
            catch (InvalidOperationException error)
            {
                return Conflict(new { message = error.Message });
            }
        }

        [Authorize(Roles = "Organiser")]
        [HttpDelete("delete/{categoryId}")]
        public IActionResult DeleteCategory(int categoryId)
        {
            var organiserClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (organiserClaim == null ||
                !int.TryParse(organiserClaim.Value, out int organiserId))
            {
                return Unauthorized();
            }

            try
            {
                bool deleted = db.DeleteCategory(categoryId, organiserId);

                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException error)
            {
                return Conflict(new { message = error.Message });
            }
        }
    }
}