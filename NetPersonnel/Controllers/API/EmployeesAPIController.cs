using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Data;
using NetPersonnel.DTOs;
using NetPersonnel.Models;
using NetPersonnel.Services;
using System;
namespace NetPersonnel.Controllers.API
{
    [Authorize]
    [ApiController]
    [Route("api/employees")]
    public class EmployeesAPIController : ControllerBase
    {
        ApplicationDBContext _db;
        private readonly IAppLogger _logger;
        public EmployeesAPIController(ApplicationDBContext db, IAppLogger logger)
        {
            _db = db;
            _logger = logger;

        }


        //GET: api/employees/get
        //Returns employees depending on the user's role
        [HttpGet("get")]
        public IActionResult GetEmployees()
        {
            if (User.IsInRole("HR") || User.IsInRole("Admin")) 
                return GetAllEmployees();

            else if (User.IsInRole("Manager"))
                return GetEmployeesByDepartment();

            else return BadRequest();
        }

        //Returns all employees with related data
        //Used only by HR
        public IActionResult GetAllEmployees()
        {
            var list = _db.Employees.Include(e => e.Department).ToList();
            return Ok(list);
        }


        //Returns employees from the manager's department
        public IActionResult GetEmployeesByDepartment()
        {
            int deptId = int.Parse(User.FindFirst("DepartmentID").Value);
            var employees = _db.Employees.Where(e => e.DepartmentId == deptId).Include(e => e.Department).ToList();
            return Ok(employees);
        }


        //POST: api/employees/add
        //Adds a new employee
        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDTO dto)
        {
            //Authorization Check
            if (!User.IsInRole("HR") && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
                return Forbid();


            // If Manager is adding an employee,
            // force the department to be the manager's department
            if (User.IsInRole("Manager"))
                dto.DepartmentId = Convert.ToInt32(User.FindFirst("DepartmentID")?.Value);
            
            
            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                JobTitle = dto.JobTitle,
                DepartmentId = dto.DepartmentId,
                Birthday = DateOnly.Parse(dto.Birthday),
                HireDate = DateOnly.Parse(dto.HireDate)
            };
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "added a new employee", employee.Id, ip, "");


            return Ok(employee);
        }


        //PUT: api/employees/edit
        //Edits an existing employee
        [HttpPut("edit")]
        public async Task<IActionResult> EditEmployee([FromBody] EmployeeDTO dto)
        {
            //Check if employee exists
            var exists = await _db.Employees.AnyAsync(e => e.Id == dto.Id);
            if (!exists)
                return NotFound();


            //Authorization Check
            if (!User.IsInRole("Manager") && !User.IsInRole("HR") && !User.IsInRole("Admin"))
                return Forbid();


            if (User.IsInRole("Manager"))
            {
                int dept = Convert.ToInt32(User.FindFirst("DepartmentID")?.Value);
                int empDept = await _db.Employees.Where(e => e.Id == dto.Id).Select(e => e.DepartmentId).FirstOrDefaultAsync();

                //Manager cannot edit employees from other departments
                if (empDept != dept)
                    return Forbid();

                //Ensure manager cannot change department
                dto.DepartmentId = Convert.ToInt32(User.FindFirst("DepartmentID")?.Value);
            }


            var employee = new Employee
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                JobTitle = dto.JobTitle,
                DepartmentId = dto.DepartmentId,
                Birthday = DateOnly.Parse(dto.Birthday),
                HireDate = DateOnly.Parse(dto.HireDate)
            };



            _db.Entry(employee).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "edited an employee", employee.Id, ip, "");


            return Ok(employee);
        }


        //DELETE: api/employees/delete
        //Deletes an existing employee
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteEmployee([FromQuery] int id)
        {
            //Check if employee exists
            bool exists = await _db.Employees.AnyAsync(e => e.Id == id);
            if (!exists)
                return NotFound();


            //Authorization Check
            if (!User.IsInRole("Manager") && !User.IsInRole("HR") && !User.IsInRole("Admin"))
                return Forbid();




            if (User.IsInRole("Manager"))
            {
                int dept = Convert.ToInt32(User.FindFirst("DepartmentID")?.Value);
                int empDept = await _db.Employees.Where(e => e.Id == id).Select(e => e.DepartmentId).FirstOrDefaultAsync();

                //Manager cannot delete employees from other departments
                if (empDept != dept)
                    return Forbid();
            }
                
            var employee = new Employee { Id = id };
            _db.Employees.Attach(employee);
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "deleted an employee", employee.Id, ip, "");

            return NoContent();
            
        }
    }
}
