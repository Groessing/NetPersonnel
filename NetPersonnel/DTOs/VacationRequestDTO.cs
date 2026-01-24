using NetPersonnel.Models;

namespace NetPersonnel.DTOs
{
    public class VacationRequestDTO
    {
        public int Id { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }


        public int StatusId { get; set; }

        
    }
}
