using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NetPersonnel.Data;
using NetPersonnel.Models;
using NetPersonnel.Tests.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
namespace NetPersonnel.Tests.Integration
{
    public class EmployeesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory> 
    {
        private readonly HttpClient _client;
        public EmployeesControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

           
        }

        [Fact]
        public async Task AddEmployee_AsAdmin_ReturnsOk()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "Admin");

            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);
            var returnedDept = await response.Content.ReadFromJsonAsync<Department>();

            var employee = new
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = returnedDept.Id,
                Birthday = "1990-12-12",
                HireDate = "2026-01-15",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };
            
          
           
          
            response = await _client.PostAsJsonAsync("/api/employees/add", employee);
            var returnedEmployee = await response.Content.ReadFromJsonAsync<Employee>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Test", returnedEmployee.FirstName);
            Assert.Equal("Test Employee", returnedEmployee.JobTitle);
        }


        [Fact]
        public async Task AddEmployee_AsUnauthorizedUser_ReturnsForbidden()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "");
         
            var employee = new
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = 1,
                Birthday = "1990-12-12",
                HireDate = "2026-01-15",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };


           


            var response = await _client.PostAsJsonAsync("/api/employees/add", employee);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        }


        [Fact]
        public async Task EditEmployee_AsAdmin_ReturnsOk()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "Admin");

            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);
            var returnedDept = await response.Content.ReadFromJsonAsync<Department>();

            var employee = new
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = returnedDept.Id,
                Birthday = "1990-12-12",
                HireDate = "2026-01-15",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };



            response = await _client.PostAsJsonAsync("/api/employees/add", employee);
            var returnedEmployee = await response.Content.ReadFromJsonAsync<Employee>();



            var editedEmployee = new
            {
                Id = returnedEmployee.Id,
                FirstName = "Test__",
                LastName = "Test",
                DepartmentId = 1,
                Birthday = "1990-12-12",
                HireDate = "2026-01-15",
                JobTitle = "Test Edited Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };
            

            response = await _client.PutAsJsonAsync("/api/employees/edit", editedEmployee);
            returnedEmployee = await response.Content.ReadFromJsonAsync<Employee>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Test__", returnedEmployee.FirstName);
            Assert.Equal("Test Edited Employee", returnedEmployee.JobTitle);
            
        }




        [Fact]
        public async Task EditEmployee_AsUnauthorizedUser_ReturnsForbidden()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "Admin");


            //Creation of Department
            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);
            var returnedDept = await response.Content.ReadFromJsonAsync<Department>();



            //Creation of Employee
            var employee = new
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = returnedDept.Id,
                Birthday = "1990-12-12",
                HireDate = "2026-01-15",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };
            response = await _client.PostAsJsonAsync("/api/employees/add", employee);
            var returnedEmployee = await response.Content.ReadFromJsonAsync<Employee>();


            _client.DefaultRequestHeaders.Remove("Test-Role");
            _client.DefaultRequestHeaders.Add("Test-Role", "");


            //Editing of Employee
            var editedEmployee = new
            {
                Id = returnedEmployee.Id,
                FirstName = "Test__",
                LastName = "Test",
                DepartmentId = returnedDept.Id,
                Birthday = "1990-12-12",
                HireDate = "2026-01-15",
                JobTitle = "Test Edited Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };


            response = await _client.PutAsJsonAsync("/api/employees/edit", editedEmployee);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        }



        [Fact]
        public async Task EditEmployee_AsAdmin_ReturnsForbidden()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "Admin");


            //Creation of Department
            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);
            var returnedDept = await response.Content.ReadFromJsonAsync<Department>();



            //Creation of Employee
            var employee = new
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = returnedDept.Id,
                Birthday = "1990-12-12",
                HireDate = "2026-01-15",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };
            response = await _client.PostAsJsonAsync("/api/employees/add", employee);
            var returnedEmployee = await response.Content.ReadFromJsonAsync<Employee>();





            response = await _client.DeleteAsync($"/api/employees/delete?id={returnedEmployee.Id}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        }


        [Fact]
        public async Task DeleteEmployee_AsUnauthorizedUser_ReturnsForbidden()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "Admin");


            //Creation of Department
            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);
            var returnedDept = await response.Content.ReadFromJsonAsync<Department>();



            //Creation of Employee
            var employee = new
            {
                FirstName = "Test",
                LastName = "Test",
                DepartmentId = returnedDept.Id,
                Birthday = "1990-12-12",
                HireDate = "2026-01-15",
                JobTitle = "Test Employee",
                Email = "test@netpersonnel.com",
                Phone = "+43 555 0000000"
            };
            response = await _client.PostAsJsonAsync("/api/employees/add", employee);
            var returnedEmployee = await response.Content.ReadFromJsonAsync<Employee>();


            _client.DefaultRequestHeaders.Remove("Test-Role");
            _client.DefaultRequestHeaders.Add("Test-Role", "");





            response = await _client.DeleteAsync($"/api/employees/delete?id={returnedEmployee.Id}");
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        }
    }
}
