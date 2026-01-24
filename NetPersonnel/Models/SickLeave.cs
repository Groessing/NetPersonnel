namespace NetPersonnel.Models
{
    public class SickLeave
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }
        public DateOnly FromDate { get; set; }

        public DateOnly ToDate { get; set; }

        public string Info { get; set; }
    }
}
