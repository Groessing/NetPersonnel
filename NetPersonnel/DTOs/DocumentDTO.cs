using NetPersonnel.Models;

namespace NetPersonnel.DTOs
{
    public class DocumentDTO
    {
        public int Id { get; set; }

        public string Filename { get; set; }

        public int? EmployeeId { get; set; }

        public int DocumentTypeId { get; set; }

        public IFormFile File { get; set; }

    }
}
