using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Data;
using NetPersonnel.DTOs;
using NetPersonnel.Models;
using NetPersonnel.Services;
using System.Xml;

namespace NetPersonnel.Controllers.API
{
    [Authorize]
    [Route("api/departments")]
    [ApiController]
    public class DepartmentsAPIController : ControllerBase
    {
        ApplicationDBContext _db;
        private readonly IAppLogger _logger;
        public DepartmentsAPIController(ApplicationDBContext db, IAppLogger logger)
        {
            _db = db;
            _logger = logger;
        
        }

        //GET: api/departments/get
        //Returns a list of all departments
        //Accessible by HR & Admin
        [HttpGet("get")]
        public async Task<IActionResult> GetDepartments()
        {
            if (!User.IsInRole("HR") && !User.IsInRole("Admin"))
                return Forbid();

            var departments = await _db.Departments.AsNoTracking().ToListAsync();
            return Ok(departments);
        }

        //POST: api/departments/add
        //Adds a new department
        //Only HR and Admin roles are allowed
        [HttpPost("add")]
        public async Task<IActionResult> AddDepartment([FromBody] Department department)
        {
            //Authorization check
            if (!User.IsInRole("HR") && !User.IsInRole("Admin"))
                return Forbid();
            
            _db.Departments.Add(department);
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "added a new department", department.Id, ip, "");
            
            return Ok(department);
        }


        //PUT: api/departments/edit
        //Updates an existing department
        //Only HR and Admin roles are allowed
        [HttpPut("edit")]
        public async Task<IActionResult> EditDepartment([FromBody] Department department)
        {
            //Authorization check
            if (!User.IsInRole("HR") && !User.IsInRole("Admin"))
                return Forbid();

            //Check whether the department exists in the database
            bool exists = await _db.Departments.AnyAsync(d => d.Id == department.Id);
            if (!exists)
                return NotFound();

            
            _db.Entry(department).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "edited a department", department.Id, ip, "");
            
            return Ok(department);
        }


        //DELETE: api/departments/delete?id=1
        //Deletes a department by its ID
        //Only HR and Admin roles are allowed
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteDepartment([FromQuery] int id)
        {
            if (!User.IsInRole("HR") && !User.IsInRole("Admin"))
                return Forbid();

            var department = new Department { Id = id };
            _db.Departments.Attach(department);
            _db.Departments.Remove(department);
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "deleted a department", id, ip, "");

            return NoContent();
        }

    }
}
