using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DbConnection;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    /*[Route("api/[controller]")]
    [ApiController]
    public class Results : ControllerBase
    {
        private readonly Db_connection db;

        public Results(Db_connection db)
        {
            this.db = db;
        }

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
    }*/
}
