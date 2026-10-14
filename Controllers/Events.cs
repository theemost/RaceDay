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
}
