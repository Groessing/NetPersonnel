using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Controllers.API;
using NetPersonnel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace NetPersonnel.Tests.Service
{
    public class ControllerRole
    {
        public EmployeesAPIController GetEmployeeControllerWithUser(DbContextOptions<ApplicationDBContext> options, string role)
        {
            var context = new ApplicationDBContext(options);
            var controller = new EmployeesAPIController(context);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, role),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("EmployeeID", "1"),
                    new Claim("UserID", "1"),
                    new Claim("DepartmentID", "1")
                }, "TestAuth"))
                }
            };

            return controller;
        }

        public DepartmentsAPIController GetDepartmentControllerWithUser(DbContextOptions<ApplicationDBContext> options, string role)
        {
            var context = new ApplicationDBContext(options);
            var controller = new DepartmentsAPIController(context);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, role),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("EmployeeID", "1"),
                    new Claim("UserID", "1"),
                    new Claim("DepartmentID", "1")
                }, "TestAuth"))
                }
            };

            return controller;
        }

        public SickLeavesAPIController GetSickLeaveControllerWithUser(DbContextOptions<ApplicationDBContext> options, string role)
        {
            var context = new ApplicationDBContext(options);
            var controller = new SickLeavesAPIController(context);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, role),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("EmployeeID", "1"),
                    new Claim("UserID", "1"),
                    new Claim("DepartmentID", "1")
                }, "TestAuth"))
                }
            };

            return controller;
        }

        public VacationRequestsAPIController GetVacationRequestControllerWithUser(DbContextOptions<ApplicationDBContext> options, string role)
        {
            var context = new ApplicationDBContext(options);
            var controller = new VacationRequestsAPIController(context);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, role),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("EmployeeID", "1"),
                    new Claim("UserID", "1"),
                    new Claim("DepartmentID", "1")
                }, "TestAuth"))
                }
            };

            return controller;
        }

        public UsersAPIController GetUserControllerWithUser(DbContextOptions<ApplicationDBContext> options, string role)
        {
            var context = new ApplicationDBContext(options);
            var controller = new UsersAPIController(context);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, role),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("EmployeeID", "1"),
                    new Claim("UserID", "1"),
                    new Claim("DepartmentID", "1")
                }, "TestAuth"))
                }
            };

            return controller;
        }



        public DocumentsAPIController GetDocumentControllerWithUser(DbContextOptions<ApplicationDBContext> options, string role) 
        {
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns("wwwroot");
            envMock.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            envMock.Setup(e => e.EnvironmentName).Returns("Development");

            var context = new ApplicationDBContext(options);
            var controller = new DocumentsAPIController(context, envMock.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, role),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("EmployeeID", "1"),
                    new Claim("UserID", "1"),
                    new Claim("DepartmentID", "1")
                }, "TestAuth"))
                }
            };

            return controller;
        }
    }
}
