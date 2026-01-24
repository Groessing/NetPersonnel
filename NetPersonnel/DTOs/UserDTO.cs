using NetPersonnel.Models;

namespace NetPersonnel.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }    // visible name
        public string Password { get; set; }
        public int RoleId { get; set; }        // e.g. "Admin", "User", "Manager"
        public int EmployeeId { get; set; }
        public bool IsActive { get; set; }

    }
}
