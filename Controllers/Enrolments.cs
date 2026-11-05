using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    /*[Route("api/[controller]")]
    [ApiController]
    public class Enrolments : ControllerBase
    {
        private readonly Db_connection db;

        public Enrolments(Db_connection db)
        {
            this.db = db;
        }

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
    }*/
}
