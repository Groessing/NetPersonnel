using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using NetPersonnel.Data;
using NetPersonnel.Models;
using NetPersonnel.Services;

namespace UserAuthentification.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDBContext _db;
        private readonly IAppLogger _logger;


        public AccountController(ApplicationDBContext db, IAppLogger logger)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _db.Users.Include(u => u.Role).Include(u => u.Employee).FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return Unauthorized("Invalid credentials (user doesnt exist)");

            if (!PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Invalid credentials (wrong password)");

            if (!user.IsActive)
                return Unauthorized("The user is set as inactive");
            
            
            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("EmployeeID", user.EmployeeId.ToString()),
                new Claim("UserID", user.Id.ToString()),
                new Claim("DepartmentID", user.Employee.DepartmentId.ToString())
            };
           

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(user.Id, "logged in", null, ip, "");

            // Redirect to role-based dashboard route
            return user.Role.Name switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Employee" => RedirectToAction("Dashboard", "Employee"),
                "HR" => RedirectToAction("Dashboard", "HR"),
                "Manager" => RedirectToAction("Dashboard", "Manager"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
