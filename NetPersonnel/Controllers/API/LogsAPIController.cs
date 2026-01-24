using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetPersonnel.Data;

namespace NetPersonnel.Controllers.API
{
    [Authorize(Roles="Admin")]
    [Route("api/logs")]
    [ApiController]
    public class LogsAPIController : Controller
    {
        ApplicationDBContext _db;
        public LogsAPIController(ApplicationDBContext db)
        {
            _db = db;
        }

        //GET: api/logs/get
        //Returns all logs
        [HttpGet("get")]
        public IActionResult GetLogs()
        {
            var list = _db.Logs.ToList();
            return Ok(list);
        }
    }
}
