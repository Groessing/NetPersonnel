using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Data;
using NetPersonnel.DTOs;
using NetPersonnel.Models;
using NetPersonnel.Tests.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPersonnel.Tests.Controllers
{
    public class DepartmentsControllerTests
    {
        [Fact]
        public async Task AddDepartment_HRUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AddDepartmentTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetDepartmentControllerWithUser(options, "HR");
            var db = new ApplicationDBContext(options);

            var result = await controller.AddDepartment(new Department
            {
               Name = "Test"
            });

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task AddDepartment_EmployeeUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
               .UseInMemoryDatabase(databaseName: "AddDepartmentTest")
               .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetDepartmentControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var result = await controller.AddDepartment(new Department
            {
                Name = "Test"
            });

            Assert.IsType<ForbidResult>(result);
        }
        [Fact]
        public async Task EditDepartment_HRUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditDepartmentTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetDepartmentControllerWithUser(options, "HR");
            var db = new ApplicationDBContext(options);

            var department = new Department
            {
                Name = "Test"
            }
            ;
            db.Departments.Add(department);
            await db.SaveChangesAsync();


            var newDb = new ApplicationDBContext(options);
            var dept = newDb.Departments.First();

            var newDept = new Department
            {
                Id = dept.Id,
                Name= "Test__"
            };

            var result = await controller.EditDepartment(newDept);



            db = new ApplicationDBContext(options);
            dept = db.Departments.First();
            Assert.Equal("Test__", dept.Name);
            Assert.IsType<OkObjectResult>(result);


        }

        [Fact]
        public async Task EditDepartment_EmployeeUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                 .UseInMemoryDatabase(databaseName: "EditDepartmentTest")
                 .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetDepartmentControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var department = new Department
            {
                Name = "Test"
            }
            ;
            db.Departments.Add(department);
            await db.SaveChangesAsync();



            var dept = db.Departments.First();

            var newDept = new Department
            {
                Name = "Test__"
            };

            var result = await controller.EditDepartment(newDept);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task EditDepartment_AdminUser_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditDepartmentTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetDepartmentControllerWithUser(options, "Admin");
            var db = new ApplicationDBContext(options);


            var department = new Department
            {
                Id = 12345,
                Name = "Test"
            };

            var result = await controller.EditDepartment(department);



            var newDB = new ApplicationDBContext(options);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteDepartment_AdminUser_ReturnsNocontent()
        {

            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "DeleteDepartmentTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetDepartmentControllerWithUser(options, "Admin");
            var db = new ApplicationDBContext(options);


            var department = new Department
            {
                Name = "Test"
            };

            db.Departments.Add(department);
            await db.SaveChangesAsync();


            var dept = db.Departments.First();
            var result = await controller.DeleteDepartment(dept.Id);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteDepartment_ManagerUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
               .UseInMemoryDatabase(databaseName: "DeleteDepartmentTest")
               .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetDepartmentControllerWithUser(options, "Manager");
            var db = new ApplicationDBContext(options);


            var department = new Department
            {
                Name = "Test"
            };

            db.Departments.Add(department);
            await db.SaveChangesAsync();


            var dept = db.Departments.First();
            var result = await controller.DeleteDepartment(dept.Id);
            Assert.IsType<ForbidResult>(result);
        }
    }
}
