using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Controllers;
using NetPersonnel.Controllers.API;
using NetPersonnel.Data;
using NetPersonnel.DTOs;
using NetPersonnel.Models;
using NetPersonnel.Tests.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NetPersonnel.Tests.Controllers
{
    public class EmployeesControllerTests
    {

        [Fact]
        public async Task AddEmployee_AdminUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AddEmployeeTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetEmployeeControllerWithUser(options, "Admin");

            var result = await controller.AddEmployee(new EmployeeDTO 
            { 
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = 1,
                Birthday = "1990-12-12",
                HireDate = "2026-01-15",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddEmployee_ManagerUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AddEmployeeTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetEmployeeControllerWithUser(options, "Manager");
            var db = new ApplicationDBContext(options);


            var result = await controller.AddEmployee(new EmployeeDTO
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = 2,   //It is intention that wrong department is set. The APIController will set the manager's department
                Birthday = "1990-12-12",
                HireDate = "2026-01-15",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            });

            var employee = db.Employees.First();
            Assert.Equal(1, employee.DepartmentId);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task EditEmployee_AdminUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditEmployeeTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetEmployeeControllerWithUser(options, "Admin");
            var db = new ApplicationDBContext(options);


            var employee = new Employee
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = 1,
                Birthday = new DateOnly(1990, 12, 12),
                HireDate = new DateOnly(2026, 01, 15),
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };

            db.Employees.Add(employee);
            await db.SaveChangesAsync();



            var emp = db.Employees.First();

            var employeeDTO = new EmployeeDTO
            {
                Id = emp.Id,
                FirstName = "Test__",
                LastName = "Test",
                DepartmentId = 1,
                Birthday = "1970-01-01",
                HireDate = "2026-01-01",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };

            var result = await controller.EditEmployee(employeeDTO);



            db = new ApplicationDBContext(options);
            emp = db.Employees.First();
            Assert.Equal("Test__", emp.FirstName);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task EditEmployee_AdminUser_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
               .UseInMemoryDatabase(databaseName: "EditEmployeeTest")
               .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetEmployeeControllerWithUser(options, "Admin");
            var db = new ApplicationDBContext(options);

            var employeeDTO = new EmployeeDTO
            {
                Id = 1234,
                FirstName = "Test__",
                LastName = "Test",
                DepartmentId = 1,
                Birthday = "1970-01-01",
                HireDate = "2026-01-01",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };

            var result = await controller.EditEmployee(employeeDTO);
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task EditEmployee_ManagerUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditEmployeeTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetEmployeeControllerWithUser(options, "Manager");
            var db = new ApplicationDBContext(options);


            var employee = new Employee
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = 2,
                Birthday = new DateOnly(1990, 12, 12),
                HireDate = new DateOnly(2026, 01, 15),
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };

            db.Employees.Add(employee);
            await db.SaveChangesAsync();



            var emp = db.Employees.First();

            var employeeDTO = new EmployeeDTO
            {
                Id = emp.Id,
                FirstName = "Test__",
                LastName = "Test",
                DepartmentId = 1,
                Birthday = "1970-01-01",
                HireDate = "2026-01-01",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };

            var result = await controller.EditEmployee(employeeDTO);

            Assert.IsType<ForbidResult>(result);
        }
        

        [Fact]
        public async Task DeleteEmployee_HRUser_ReturnsNoContent()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "DeleteEmployeeTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetEmployeeControllerWithUser(options, "HR");
            var db = new ApplicationDBContext(options);


            var employee = new Employee
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = 2,
                Birthday = new DateOnly(1990, 12, 12),
                HireDate = new DateOnly(2026, 01, 15),
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };

            db.Employees.Add(employee);
            await db.SaveChangesAsync();


            var emp = db.Employees.First();
            var result = await controller.DeleteEmployee(emp.Id);
            Assert.IsType<NoContentResult>(result);
        }



        [Fact]
        public async Task DeleteEmployee_ManagerUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "DeleteEmployeeTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetEmployeeControllerWithUser(options, "Manager");
            var db = new ApplicationDBContext(options);


            var employee = new Employee
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = 2,
                Birthday = new DateOnly(1990, 12, 12),
                HireDate = new DateOnly(2026, 01, 15),
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };

            db.Employees.Add(employee);
            await db.SaveChangesAsync();


            var emp = db.Employees.ToList()[0];
            var result = await controller.DeleteEmployee(emp.Id);
            Assert.IsType<ForbidResult>(result);
        }
    }
}
