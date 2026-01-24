using Microsoft.EntityFrameworkCore;
using NetPersonnel.Models;
using System.Collections.Generic;
namespace NetPersonnel.Data
{
    public class ApplicationDBContext : DbContext
    {
        
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> opts) : base(opts)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<SickLeave> SickLeaves { get; set; }
        public DbSet<VacationRequest> VacationRequests { get; set; }

        public DbSet<VacationStatus> VacationStatus { get; set; } //Used in Integration Test only

        public DbSet<Log> Logs { get; set; }

        public DbSet<Document> Documents { get; set; }


        public DbSet<DocumentType> DocumentTypes { get; set; }


    }
}
