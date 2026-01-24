using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Data;
using NetPersonnel.Models;

namespace NetPersonnel.Controllers.API
{
    [Authorize]
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardAPIController : ControllerBase
    {
        ApplicationDBContext _db;
        public DashboardAPIController(ApplicationDBContext db)
        {
            _db = db;

        }




        [HttpGet("get")]
        public async Task<IActionResult> GetCounts()
        {
            Dashboard dashboard = new Dashboard();

            /*
             =========================
                    HR DASHBOARD
             =========================
            HR can see global company-wide statistics
            */
            if (User.IsInRole("HR"))
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var last30Days = today.AddDays(-30);

                dashboard.EmployeesCount = await _db.Employees.CountAsync();
                dashboard.SickLeavesLast30DaysCount = await _db.SickLeaves.Where(s => s.ToDate >= last30Days).CountAsync();
                dashboard.PendingVacationRequestsCount = await _db.VacationRequests.Where(v => v.StatusId == 1).CountAsync();
                dashboard.DepartmentsCount = await _db.Departments.CountAsync();
                dashboard.DocumentsCount = await _db.Documents.CountAsync();

                return Ok(dashboard);

            }


            /*
             =========================
                MANAGER DASHBOARD
             =========================
            Managers can only see data related to their department
            */
            else if (User.IsInRole("Manager"))
            {
                int deptId = int.Parse(User.FindFirst("DepartmentID").Value);
                var today = DateOnly.FromDateTime(DateTime.Today);
                var last30Days = today.AddDays(-30);

                dashboard.EmployeesCount = await _db.Employees.Where(e => e.DepartmentId == deptId).CountAsync();
                dashboard.SickLeavesLast30DaysCount = await _db.SickLeaves.Where(s => s.Employee.DepartmentId == deptId).Where(s => s.ToDate >= last30Days).CountAsync();
                dashboard.PendingVacationRequestsCount = await _db.VacationRequests.Where(s => s.Employee.DepartmentId == deptId).Where(v => v.StatusId == 1).CountAsync();
                dashboard.DocumentsCount = await _db.Documents.Where(d => d.Employee.DepartmentId == deptId).CountAsync();

                return Ok(dashboard);
            }


            /*
             =========================
                ADMIN DASHBOARD
             =========================
             Admin focuses on system-level data
            */
            else if (User.IsInRole("Admin"))
            {

                dashboard.EmployeesCount = await _db.Employees.CountAsync();
                dashboard.DepartmentsCount = await _db.Departments.CountAsync();
                dashboard.ActiveUsersCount = await _db.Users.Where(u => u.IsActive == true).CountAsync();

                return Ok(dashboard);

            }

            /*
            =========================
                EMPLOYEE DASHBOARD
            =========================
            Employees only see their own data
            */
            else if (User.IsInRole("Employee"))
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var last30Days = today.AddDays(-30);
                int empId = int.Parse(User.FindFirst("EmployeeID").Value);
                dashboard.SickLeavesLast30DaysCount = await _db.SickLeaves.Where(s => s.EmployeeId == empId).Where(s => s.ToDate >= last30Days).CountAsync();
                dashboard.PendingVacationRequestsCount = await _db.VacationRequests.Where(v => v.EmployeeId == empId).Where(v => v.StatusId == 1).CountAsync();
                dashboard.DocumentsCount = await _db.Documents.Where(v => v.EmployeeId == empId).CountAsync();

                return Ok(dashboard);
            }

            else
            {
                return BadRequest();
            }


        }

        
    }
}
