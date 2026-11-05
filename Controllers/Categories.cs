using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    /*[Route("api/[controller]")]
    [ApiController]
    public class Categories : ControllerBase
    {

        private readonly Db_connection db;

        public Results(Db_connection db)
        {
            this.db = db;
        }

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
    }*/
}
