using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Data;
using NetPersonnel.Models;
using NetPersonnel.DTOs;
using NetPersonnel.Tests.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPersonnel.Tests.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task AddUser_AdminUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AddUserTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetUserControllerWithUser(options, "Admin");
            var db = new ApplicationDBContext(options);

            var result = await controller.AddUser(new UserDTO
            {
                Username = "Test",
                Password = "Test",
                RoleId = 1,
                IsActive = true,
                EmployeeId = 1,
            });

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task AddUser_EmployeeUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AddUserTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetUserControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var result = await controller.AddUser(new UserDTO
            {
                Username = "Test",
                Password = "Test",
                RoleId = 1,
                IsActive = true,
                EmployeeId = 1,
            });

            Assert.IsType<ForbidResult>(result);
        }


        [Fact]
        public async Task EditStatus_AdminUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditUserTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetUserControllerWithUser(options, "Admin");
            var db = new ApplicationDBContext(options);

            var user = new User
            {
                Username = "Test",
                PasswordSalt = new byte[] { 1, 2, 3, 4, 5 },
                PasswordHash = new byte[] { 1, 2, 3, 4, 5 },
                RoleId = 1,
                IsActive = true,
                EmployeeId = 1,
            }
            ;
            db.Users.Add(user);
            await db.SaveChangesAsync();


           
            
            int userId = db.Users.Select(u => u.Id).First();

            
            var result = await controller.EditStatus(userId, false);



            db = new ApplicationDBContext(options);
            user = db.Users.First();


            Assert.False(user.IsActive);
            Assert.IsType<OkObjectResult>(result);


        }

        [Fact]
        public async Task EditStatus_EmployeeUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "EditUserTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetUserControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var user = new User
            {
                Username = "Test",
                PasswordSalt = new byte[] { 1, 2, 3, 4, 5 },
                PasswordHash = new byte[] { 1, 2, 3, 4, 5 },
                RoleId = 1,
                IsActive = true,
                EmployeeId = 1,
            }
            ;
            db.Users.Add(user);
            await db.SaveChangesAsync();


            int userId = db.Users.Select(u => u.Id).First();
            var result = await controller.EditStatus(userId, false);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task EditStatus_AdminUser_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
               .UseInMemoryDatabase(databaseName: "EditUserTest")
               .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetUserControllerWithUser(options, "Admin");
            var db = new ApplicationDBContext(options);



            var result = await controller.EditStatus(1, false);



            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUser_AdminUser_ReturnsNocontent()
        {

            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
              .UseInMemoryDatabase(databaseName: "DeleteDepartmentTest")
              .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetUserControllerWithUser(options, "Admin");
            var db = new ApplicationDBContext(options);


            var user = new User
            {
                Username = "Test",
                PasswordSalt = new byte[] { 1, 2, 3, 4, 5 },
                PasswordHash = new byte[] { 1, 2, 3, 4, 5 },
                RoleId = 1,
                IsActive = true,
                EmployeeId = 1,
            }
            ;
            db.Users.Add(user);
            await db.SaveChangesAsync();


            user = db.Users.First();
            var result = await controller.DeleteUser(user.Id);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_EmployeeUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
               .UseInMemoryDatabase(databaseName: "DeleteDepartmentTest")
               .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetUserControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);


            var user = new User
            {
                Username = "Test",
                PasswordSalt = new byte[] { 1, 2, 3, 4, 5 },
                PasswordHash = new byte[] { 1, 2, 3, 4, 5 },
                RoleId = 1,
                IsActive = true,
                EmployeeId = 1,
            }
            ;
            db.Users.Add(user);
            await db.SaveChangesAsync();


            user = db.Users.First();
            var result = await controller.DeleteUser(user.Id);
            Assert.IsType<ForbidResult>(result);
        }
    }
}
