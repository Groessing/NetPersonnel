using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    public class SickLeavesControllerTests
    {

        [Fact]
        public async Task AddSickLeave_EmployeeUser_ReturnsOk()
        {

            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AddSickLeaveTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetSickLeaveControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var result = await controller.AddSickLeave(new SickLeaveDTO
            {
                EmployeeId = -1,
                FromDate = "2026-01-18",
                ToDate = "2026-01-30",
                Info = "Test"
            });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddSickLeave_ManagerUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AddSickLeaveTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetSickLeaveControllerWithUser(options, "Maanger");
            var db = new ApplicationDBContext(options);

            var result = await controller.AddSickLeave(new SickLeaveDTO
            {
                FromDate = "2026-01-18",
                ToDate = "2026-01-30",
                Info = "Test"
            });

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task AddSickLeave_EmployeeUser_ReturnsBadRequest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AddSickLeaveTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetSickLeaveControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var result = await controller.AddSickLeave(new SickLeaveDTO
            {
                FromDate = "2026-01-30",
                ToDate = "2026-01-18",
                Info = "Test"
            });

            Assert.IsType<BadRequestResult>(result);
        }


        [Fact]
        public async Task EditSickLeave_HRUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditSickLeaveTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetSickLeaveControllerWithUser(options, "HR");
            var db = new ApplicationDBContext(options);

            var employee = new Employee
            {
                Id = 2,
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

            var sickLeave = new SickLeave
            {
                FromDate = new DateOnly(2026, 01, 18),
                ToDate = new DateOnly(2026, 01, 30),
                EmployeeId = 2,
                Info = ""
            };
            db.SickLeaves.Add(sickLeave);
            await db.SaveChangesAsync();



            int sickLeaveId = db.SickLeaves.Select(s => s.Id).First();


            var editedSickLeave = new SickLeaveDTO
            {
                Id = sickLeaveId,
                FromDate = "2025-01-18",
                ToDate = "2026-01-30",
                EmployeeId = 2,
                Info = "Test"
            };
            var result = await controller.EditSickLeave(editedSickLeave);


            db = new ApplicationDBContext(options);
            var request = db.SickLeaves.First();

            Assert.Equal("Test", request.Info);
            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task EditSickLeave_HRUser_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditSickLeaveTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetSickLeaveControllerWithUser(options, "HR");
            var db = new ApplicationDBContext(options);

            var editedSickLeave = new SickLeaveDTO
            {
                Id = 1,
                FromDate = "2025-01-18",
                ToDate = "2026-01-30",
                EmployeeId = 2,
                Info = ""
            };
            var result = await controller.EditSickLeave(editedSickLeave);
            Assert.IsType<NotFoundResult>(result);
        }



        [Fact]
        public async Task EditSickLeave_ManagerUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditSickLeaveTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetSickLeaveControllerWithUser(options, "Manager");
            var db = new ApplicationDBContext(options);

            var editedSickLeave = new SickLeaveDTO
            {
                Id = 1,
                FromDate = "2025-01-18",
                ToDate = "2026-01-30",
                EmployeeId = 2,
                Info = ""
            };
            var result = await controller.EditSickLeave(editedSickLeave);
            Assert.IsType<ForbidResult>(result);
        }
    }
}
