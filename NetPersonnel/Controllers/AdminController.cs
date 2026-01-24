using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Models;
using System.Security.Claims;
using NetPersonnel.Data;
using Microsoft.AspNetCore.Authorization;

namespace NetPersonnel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDBContext _db;

        public AdminController(ApplicationDBContext db)
        {
           _db = db;
        }
        public IActionResult Dashboard()
        {
            
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

        public IActionResult Employees()
        {
            return View();
        }

        public IActionResult Departments()
        {
            return View();
        }

        public IActionResult Logs()
        {
            return View();
        }


    }
}
