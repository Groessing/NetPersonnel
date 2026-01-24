namespace NetPersonnel.Models
{
    public class Log
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }   

        public string Action { get; set; }
        public DateTime TimeStamp { get; set; }

        //It is not a foreign key because it can reference to every model
        public int? ObjectId { get; set;  }

        public string IPAddress { get; set; }

        public string Details { get; set; }
    }
}
