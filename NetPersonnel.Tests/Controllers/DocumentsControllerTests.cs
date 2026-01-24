using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Data;
using NetPersonnel.DTOs;
using NetPersonnel.Models;
using NetPersonnel.Tests.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace NetPersonnel.Tests.Controllers
{
    public class DocumentsControllerTests
    {
        [Fact]
        public async Task UploadDocument_HRUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "UploadDocumentTest")
                .Options;

            ControllerRole role = new ControllerRole();

            var controller = role.GetDocumentControllerWithUser(options, "HR");
            var db = new ApplicationDBContext(options);

            var file = MockFile.CreateMockFile();
            var result = await controller.UploadDocument(new DocumentDTO
            {
                Filename = "test.pdf",
                DocumentTypeId = 1,
                File = file
            });

            Assert.IsType<OkObjectResult>(result);

        }



        [Fact]
        public async Task UploadDocument_ManagerUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "UploadDocumentTest")
                .Options;

            ControllerRole role = new ControllerRole();
            var controller = role.GetDocumentControllerWithUser(options, "Manager");
            var db = new ApplicationDBContext(options);

            var file = MockFile.CreateMockFile();
            var result = await controller.UploadDocument(new DocumentDTO
            {
                Filename = "test.pdf",
                EmployeeId = 1,
                DocumentTypeId = 1,
                File = file
            });

            Assert.IsType<ForbidResult>(result);

        }



        [Fact]
        public async Task DownloadDocument_EmployeeUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "DownloadDocumentTest")
                .Options;

            ControllerRole role = new ControllerRole();
            
            var controller = role.GetDocumentControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var file = MockFile.CreateMockFile();
            await controller.UploadDocument(new DocumentDTO
            {
                Filename = "test.pdf",
                DocumentTypeId = 1,
                File = file
            });

            db = new ApplicationDBContext(options);
           
            var documentId = db.Documents.Select(d => d.Id).First();
            var result = await controller.DownloadDocument(documentId);

            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal("test.pdf", fileResult.FileDownloadName);
            Assert.NotEmpty(fileResult.FileContents);

        }



        [Fact]
        public async Task DownloadDocument_ManagerUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "DownloadDocumentTest")
                .Options;

            ControllerRole role = new ControllerRole();

            var controller = role.GetDocumentControllerWithUser(options, "Manager");
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
            
            
            Models.Document document = new Models.Document
            {
                Filename = "test.pdf",
                FilePath = "Documents/Employee",
                UploadedBy = 2,
                EmployeeId = 2,
                DocumentTypeId = 1,
                UploadedAt = DateTime.Now,
                ContentType = "application/pdf"
            
            };

            db.Documents.Add(document);
            await db.SaveChangesAsync();

            db = new ApplicationDBContext(options);

            var documentId = db.Documents.Select(d => d.Id).First();
            var result = await controller.DownloadDocument(documentId);

            Assert.IsType<ForbidResult>(result);


        }


        [Fact]
        public async Task DownloadDocument_EmployeeUser_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "DownloadDocumentTest")
                .Options;

            ControllerRole role = new ControllerRole();

            var controller = role.GetDocumentControllerWithUser(options, "Employee");


            var result = await controller.DownloadDocument(100);

            Assert.IsType<NotFoundResult>(result);

        }


        [Fact]
        public async Task DeleteDocument_EmployeeUser_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "DeleteDocumentTest")
                .Options;

            ControllerRole role = new ControllerRole();

            var controller = role.GetDocumentControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);

            var employee = new Employee
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
            };
            db.Employees.Add(employee);
            await db.SaveChangesAsync();



            var file = MockFile.CreateMockFile();
            await controller.UploadDocument(new DocumentDTO
            {
                Filename = "test.pdf",
                DocumentTypeId = 1,
                File = file
            });

            db = new ApplicationDBContext(options);

            var documentId = db.Documents.Select(d => d.Id).First();
            var result = await controller.DeleteDocument(documentId);

            Assert.IsType<NoContentResult>(result);


        }


        [Fact]
        public async Task DeleteDocument_EmployeeUser_ReturnsForbid()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "DeleteDocumentTest")
                .Options;

            ControllerRole role = new ControllerRole();

            var controller = role.GetDocumentControllerWithUser(options, "Employee");
            var db = new ApplicationDBContext(options);
            

            Models.Document document = new Models.Document
            {
                Filename = "test.pdf",
                FilePath = "Documents/Employee",
                UploadedBy = 10,
                EmployeeId = 2,
                DocumentTypeId = 1,
                UploadedAt = DateTime.Now,
                ContentType = "application/pdf"

            };

            db.Documents.Add(document);


            await db.SaveChangesAsync();


            var documentId = db.Documents.Select(d => d.Id).First();
            var result = await controller.DeleteDocument(documentId);

            Assert.IsType<ForbidResult>(result);


        }




        [Fact]
        public async Task DeleteDocument_EmployeeUser_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "DeleteDocumentTest")
                .Options;

            ControllerRole role = new ControllerRole();

            var controller = role.GetDocumentControllerWithUser(options, "Employee");
            var result = await controller.DeleteDocument(1);

            Assert.IsType<NotFoundResult>(result);


        }
    }
}
