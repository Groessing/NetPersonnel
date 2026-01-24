using System.ComponentModel.DataAnnotations.Schema;

namespace NetPersonnel.Models
{
    //[Table("Roles")]
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
