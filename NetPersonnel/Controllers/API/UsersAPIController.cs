using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Data;
using NetPersonnel.DTOs;
using NetPersonnel.Models;
using NetPersonnel.Services;
namespace NetPersonnel.Controllers.API
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersAPIController : ControllerBase
    {
        ApplicationDBContext _db;
        private readonly IAppLogger _logger;
        public UsersAPIController(ApplicationDBContext db, IAppLogger logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users.AsNoTracking()
                                        .Include(u => u.Role)
                                        .Include(u => u.Employee)
                                        .ToListAsync();
           
            return Ok(users);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] UserDTO dto)
        {
            if (!User.IsInRole("Admin")) return Forbid();
            /*if (_db.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return BadRequest("User exists");
            }*/

            PasswordHasher.CreatePasswordHash(dto.Password, out var hash, out var salt);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleId = dto.RoleId,
                IsActive = true,
                EmployeeId = dto.EmployeeId,
                CreatedOn = DateOnly.FromDateTime(DateTime.Now)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "added a new user", user.Id, ip, "");


            return Ok(user);
        }

        [HttpPatch("edit")]
        public async Task<IActionResult> EditStatus([FromQuery] int userId, [FromQuery] bool isActive)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            bool exists = await _db.Users.AnyAsync(u => u.Id == userId);
            if (!exists)
                return NotFound();


            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            user.IsActive = isActive;
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "changed a user's status ", user.Id, ip, "");


            return Ok(user);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            if (!User.IsInRole("Admin")) return Forbid();
            var user = new User { Id = id };
            _db.Users.Attach(user);
            _db.Users.Remove(user);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
            }

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "deleted a user", id, ip, "");


            return NoContent();
        }


    }
}
