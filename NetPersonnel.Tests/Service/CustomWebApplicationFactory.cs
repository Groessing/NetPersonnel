using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NetPersonnel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NetPersonnel.Tests.Service
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection _connection;
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            builder.UseEnvironment("Development");

            builder.ConfigureServices(services =>
            {
                services.Remove(
                services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDBContext>))
                );

                services.AddDbContext<ApplicationDBContext>(options =>
                     //options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
                     //options.UseInMemoryDatabase("IntegrationTestDb", _root));
                     options.UseSqlite(_connection));

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                db.Database.EnsureCreated();


                //Used to create Users
                db.Roles.AddRange(
                    new Models.Role{Id = 1, Name = "Admin"},
                    new Models.Role{Id = 2, Name = "Employee"},
                    new Models.Role {Id = 3, Name = "HR" }
                    );
               

                //Used to create Vacation Requests
                db.VacationStatus.AddRange(
                    new Models.VacationStatus { ApprovalStatus = "Pending" },
                    new Models.VacationStatus { ApprovalStatus = "Approved" },
                    new Models.VacationStatus { ApprovalStatus = "Declined" }
                    );

                
                


                /*
                This will be used for uploading documents
                First, a Department has to be created
                Second, an Employee can be created and uses the Department's ID as FK
                Third, an User can be created which uses the Employee's ID as FK
                Now the APIController can add Document to DB with UploadedBy referring to User's ID
                */

                //Creation of Department
                db.Departments.Add(new Models.Department
                {
                    Id = 1,
                    Name = "Test Department"
                });


                //Creation of Employee (after Department was created)
                db.Employees.Add(new Models.Employee
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Test",
                    DepartmentId = 1,
                    Birthday = new DateOnly(1990, 12, 12),
                    HireDate = new DateOnly(2026, 01, 15),
                    JobTitle = "Test Employee",
                    Email = "test@netpersonnel.com",
                    Phone = "+43 555 0000000"
                });



                //Used for uploading documents (createdBy FK)
                //Creation of User (after Employee was created)
                db.Users.Add(new Models.User
                 {
                     Id = 1,
                     Username = "Test",
                     RoleId = 3,
                     EmployeeId = 1,
                     PasswordHash = new byte[] { 1, 2, 3 },
                     PasswordSalt = new byte[] { 1, 2, 3 },
                     CreatedOn = DateOnly.FromDateTime(DateTime.Now)
                 });

                //Creation of DocumentTypes
                db.DocumentTypes.AddRange(
                    new Models.DocumentType { Id = 1, Name = "Test 1" },
                    new Models.DocumentType { Id = 2, Name = "Test 2" }
                    );
                db.SaveChanges();


                services.RemoveAll<IAuthenticationSchemeProvider>();

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

                services.AddAuthorization();
            });

           
        }
    }
}
