using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Data;
using NetPersonnel.DTOs;
using NetPersonnel.Models;
using NetPersonnel.Services;
using System.Globalization;
namespace NetPersonnel.Controllers.API
{
    [Authorize]
    [ApiController]
    [Route("api/sickleaves")]
    public class SickLeavesAPIController : ControllerBase
    {
        ApplicationDBContext _db;
        private readonly IAppLogger _logger;
        public SickLeavesAPIController(ApplicationDBContext db, IAppLogger logger)
        {
            _db = db;
            _logger = logger;

        }

        //GET: api/sickleaves/get
        //Returns sick leaves depending on the user's role
        [HttpGet("get")]
        public IActionResult GetSickLeaves()
        {
            if (User.IsInRole("HR"))
                return Ok(GetAllSickLeaves());

            else if (User.IsInRole("Manager"))
                return Ok(GetSickLeavesByDepartment());

            else if (User.IsInRole("Employee"))
                return Ok(GetSickLeavesByEmployee());

            else return BadRequest();
            
        }


        //Returns all employees with related data
        //Used only by HR
        public IActionResult GetAllSickLeaves()
        {
            var list = _db.SickLeaves.Include(s => s.Employee).ToList();
            return Ok(list);
        }

        //Returns sick leaves of employees from the manager's department
        public IActionResult GetSickLeavesByDepartment()
        {
            var deptId = int.Parse(User.FindFirst("DepartmentID").Value);
            var sickLeaves = _db.SickLeaves.Where(s => s.Employee.DepartmentId == deptId).Include(s => s.Employee.Department).ToList();
            return Ok(sickLeaves);
        }

        //Returns sick leaves of the current employee
        public IActionResult GetSickLeavesByEmployee()
        {

            var employeeIdClaim = User.FindFirst("EmployeeID");
            
            if (employeeIdClaim == null)
                return BadRequest("EmployeeID claim missing.");

            var employeeId = int.Parse(employeeIdClaim.Value);

            var sickLeaves = _db.SickLeaves.Where(s => s.EmployeeId == employeeId).Include(s => s.Employee).ToList();

            return Ok(sickLeaves);
        }


        //POST: api/sickleaves/add
        //Adds a new departments
        [HttpPost("add")]
        public async Task<IActionResult> AddSickLeave([FromBody] SickLeaveDTO dto)
        {
            //Authorization Check
            if (!User.IsInRole("HR") && !User.IsInRole("Employee"))
                return Forbid();
            
            var fromDate = DateTime.ParseExact(dto.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var toDate = DateTime.ParseExact(dto.ToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (fromDate > toDate)
                return BadRequest();

           

            if (User.IsInRole("Employee"))
                dto.EmployeeId = Convert.ToInt32(User.FindFirst("EmployeeID").Value);

            var sickLeave = new SickLeave
            {
                EmployeeId = dto.EmployeeId,
                FromDate = DateOnly.Parse(dto.FromDate),
                ToDate = DateOnly.Parse(dto.ToDate),
                Info = dto.Info,
               
            };
            _db.SickLeaves.Add(sickLeave);


            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
            }

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "added a new sick leave", sickLeave.Id, ip, "");


            return Ok(sickLeave);

            
        }






        //PUT: api/sickleaves/edit
        //Edits an existing sick leave
        [HttpPut("edit")]
        public async Task<IActionResult> EditSickLeave([FromBody] SickLeaveDTO dto)
        {
            //Authorization Check
            if (!User.IsInRole("HR") && !User.IsInRole("Employee"))
                return Forbid();


            //Check if sick leave exists
            var sickLeave = await _db.SickLeaves.FindAsync(dto.Id);
            if (sickLeave == null)
                return NotFound();

            sickLeave.FromDate = DateOnly.Parse(dto.FromDate);
            sickLeave.ToDate = DateOnly.Parse(dto.ToDate);
            sickLeave.Info = dto.Info;


            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "edited a sick leave", sickLeave.Id, ip, "");


            return Ok(sickLeave);
        }




    }
}
