using System.ComponentModel.DataAnnotations.Schema;

namespace NetPersonnel.Models
{
    public class Document
    {
        public int Id { get; set; }

        public string Filename { get; set; }

        public string FilePath { get; set; }


        public int UploadedBy { get; set; }

        [ForeignKey("UploadedBy")]
        public User User { get; set; }
        public int? EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public int DocumentTypeId { get; set; }

        public DocumentType DocumentType { get; set; }

        public DateTime UploadedAt { get; set; }

        public string ContentType { get; set; }
    }
}
