namespace NetPersonnel.Models
{
    public class VacationRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public DateOnly FromDate { get; set; }

        public DateOnly ToDate { get; set; }


        public int StatusId { get; set; }

        public VacationStatus Status { get; set; }
    }
}
