namespace NetPersonnel.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string JobTitle { get; set; }
        public string Birthday { get; set; }
        public string HireDate { get; set; }
        public int DepartmentId { get; set; }
    }
}
