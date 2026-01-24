using NetPersonnel.Models;

namespace NetPersonnel.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }    // visible name
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int RoleId { get; set; }        // e.g. "Admin", "User", "Manager"

        public Role Role { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public bool IsActive { get; set; }

        public DateOnly CreatedOn { get; set; }

    }
}
