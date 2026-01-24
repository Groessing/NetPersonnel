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
    [Route("api/vacationrequests")]
    public class VacationRequestsAPIController : ControllerBase
    {
        ApplicationDBContext _db;
        private readonly IAppLogger _logger;
        public VacationRequestsAPIController(ApplicationDBContext db, IAppLogger logger)
        {
            _db = db;
            _logger = logger;
        }



        //GET: api/vacationrequests/get
        //Returns vacation requests depending on the user's role
        [HttpGet("get")]
        public IActionResult GetRequests()
        {
            if (User.IsInRole("HR")) return Ok(GetAllRequests());

            else if (User.IsInRole("HR")) return Ok(GetRequestsByDepartment());

            else if (User.IsInRole("Employee")) return Ok(GetRequestsByEmployee());

            else return BadRequest();
        }


        //Returns all vacation requests with related data
        //Used only by HR
        public IActionResult GetAllRequests()
        {
            var list = _db.VacationRequests.Include(v => v.Employee).Include(v => v.Status).ToList();
            return Ok(list);
        }


        //Returns vacation requests of employees from the manager's department
        public IActionResult GetRequestsByDepartment()
        {
            var deptId = int.Parse(User.FindFirst("DepartmentID").Value);
            var requests = _db.VacationRequests.Where(v => v.Employee.DepartmentId == deptId).Include(v => v.Employee).Include(v => v.Status).ToList();
            return Ok(requests);
        }


        //Returns vacation request of current employee
        public IActionResult GetRequestsByEmployee()
        {
            var employeeIdClaim = User.FindFirst("EmployeeID");
            if (employeeIdClaim == null)
                return BadRequest("EmployeeID claim missing.");

            var employeeId = int.Parse(employeeIdClaim.Value);


            var requests = _db.VacationRequests.Where(v => v.Employee.Id == employeeId).Include(v => v.Employee).Include(v => v.Status).ToList();
            return Ok(requests);
        }


        //POST: api/vacationrequests/add
        //Adds a new vacation request
        [HttpPost("add")]
        public async Task<IActionResult> AddVacationRequest([FromBody] VacationRequestDTO dto)
        {
            //Authorization Check
            if (!User.IsInRole("Employee"))
                return Forbid();


            var fromDate = DateTime.ParseExact(dto.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var toDate = DateTime.ParseExact(dto.ToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (fromDate > toDate)
                return BadRequest();



            var vacationRequest = new VacationRequest
            {
                EmployeeId = Convert.ToInt32(User.FindFirst("EmployeeID").Value),
                FromDate = DateOnly.Parse(dto.FromDate),
                ToDate = DateOnly.Parse(dto.ToDate),
                StatusId = 1,  //Pending when created
            };
          
            _db.VacationRequests.Add(vacationRequest);
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "added a new vacation request", vacationRequest.Id, ip, "");

            return Ok(vacationRequest);
        }


        //PATCH: api/vacationrequests/edit?requestId=1&statusId=1
        //Edits the status to approve or decline
        [HttpPatch("edit")]
        public async Task<IActionResult> EditStatus([FromQuery] int requestId, [FromQuery] int statusId)
        {
            //Checks if vacation request exists
            bool exists = await _db.VacationRequests.AnyAsync(d => d.Id == requestId);
            if (!exists)
                return NotFound();

            //Authorization Check
            if (!User.IsInRole("Manager") && !User.IsInRole("HR"))
                return Forbid();

            if (User.IsInRole("Manager"))
            {
               int deptId = int.Parse(User.FindFirst("DepartmentID").Value);
               int empDeptId = await _db.VacationRequests.Where(d => d.Id == requestId).Select(d => d.Employee.DepartmentId).FirstOrDefaultAsync();

                //Manager cannot edit a vacation request of an employee from another department
                if (deptId != empDeptId)
                    return Forbid();


            }
            var vacationRequest = await _db.VacationRequests.FindAsync(requestId);
            vacationRequest.StatusId = statusId;
            await _db.SaveChangesAsync();


            string status = statusId == 2 ? "approved" : "declined";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), $"{status} a vacation request", vacationRequest.Id, ip, "");


            return Ok(vacationRequest);
        }
    }
}

