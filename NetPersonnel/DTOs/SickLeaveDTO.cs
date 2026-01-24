using NetPersonnel.Models;

namespace NetPersonnel.DTOs
{
    public class SickLeaveDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string Info { get; set; }


    }
}
