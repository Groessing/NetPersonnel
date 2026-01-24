using System.ComponentModel.DataAnnotations.Schema;
namespace NetPersonnel.Models
{
    //[Table("Employees")]
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public DateOnly Birthday { get; set; }

        public DateOnly HireDate { get; set; }

        public string JobTitle { get; set; }
    }
}
