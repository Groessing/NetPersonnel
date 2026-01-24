using Microsoft.AspNetCore.Http.HttpResults;
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
    public class VacationRequestsControllerTests
    {
        [Fact]
        public async Task AddRequest_EmployeeUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AddVacationRequestTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetVacationRequestControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var result = await controller.AddVacationRequest(new VacationRequestDTO
            {
               FromDate = "2026-01-18",
               ToDate = "2026-01-30"
            });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddRequest_EmployeeUser_BadRequest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AddVacationRequestTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetVacationRequestControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var result = await controller.AddVacationRequest(new VacationRequestDTO
            {
                FromDate = "2026-01-30",
                ToDate = "2026-01-18"
            });

            Assert.IsType<BadRequestResult>(result);
        }


        [Fact]
        public async Task EditStatus_ManagerUser_ReturnsNoContent()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditStatusTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetVacationRequestControllerWithUser(options, "Manager");
            var db = new ApplicationDBContext(options);

            var employee = new Employee
            {
                Id = 1000,
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

            var vacationRequest = new VacationRequest
            {
                Id = 1000,
                FromDate = new DateOnly(2025, 01, 18),
                ToDate = new DateOnly(2026, 01, 30),
                StatusId = 1,
                EmployeeId = 1000
            };
            db.VacationRequests.Add(vacationRequest);
            await db.SaveChangesAsync();



            int requestId = db.VacationRequests.Select(v => v.Id).First();

            var result = await controller.EditStatus(requestId, 2);


            db = new ApplicationDBContext(options);
            var request = db.VacationRequests.Where(v => v.Id == 1000).FirstOrDefault();


            Assert.Equal(2, request.StatusId);

            Assert.IsType<OkObjectResult>(result);
        }





        [Fact]
        public async Task EditStatus_EmployeeUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditStatusTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetVacationRequestControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var vacationRequest = new VacationRequest
            {
                FromDate = new DateOnly(2025, 01, 18),
                ToDate = new DateOnly(2026, 01, 30),
                StatusId = 1,
                EmployeeId = 2
            };
            db.VacationRequests.Add(vacationRequest);
            await db.SaveChangesAsync();


            int requestId = db.VacationRequests.Select(v => v.Id).First();

            var result = await controller.EditStatus(requestId, 3);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task EditStatus_ManagerUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditStatusTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetVacationRequestControllerWithUser(options, "Manager");
            var db = new ApplicationDBContext(options);


            var employee = new Employee
            {
                Id = 2,
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = 10,
                Birthday = new DateOnly(1990, 12, 12),
                HireDate = new DateOnly(2026, 01, 15),
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };
            db.Employees.Add(employee);


            var VacationRequest = new VacationRequest
            {
                FromDate = new DateOnly(2025, 01, 18),
                ToDate = new DateOnly(2026, 01, 30),
                StatusId = 1,
                EmployeeId = 2
            };
            db.VacationRequests.Add(VacationRequest);
            await db.SaveChangesAsync();



            int requestId = db.VacationRequests.Select(v => v.Id).First();

            var result = await controller.EditStatus(requestId, 3);

            Assert.IsType<ForbidResult>(result);
        }


    }
}
