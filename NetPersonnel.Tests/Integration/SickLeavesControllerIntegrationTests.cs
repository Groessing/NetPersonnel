using NetPersonnel.Models;
using NetPersonnel.Tests.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
namespace NetPersonnel.Tests.Integration
{
    public class SickLeavesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SickLeavesControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
        _client = factory.CreateClient();
        }


        [Fact]
        public async Task AddickLeave_AsHR_ReturnsOk()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "HR");

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


            //Creation of Sick Leave
            var sickLeave = new
            {
                EmployeeId = returnedEmployee.Id,
                FromDate = "2026-01-19",
                ToDate = "2026-01-20",
                Info = "Test"
            };
            response = await _client.PostAsJsonAsync("/api/sickleaves/add", sickLeave);
            var returnedSickLeave = await response.Content.ReadFromJsonAsync<SickLeave>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Test", returnedSickLeave.Info);

        }


        [Fact]
        public async Task EditSickLeave_AsHR_ReturnsOk()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "HR");

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


            //Creation of Sick Leave
            var sickLeave = new
            {
                EmployeeId = returnedEmployee.Id,
                FromDate = "2026-01-19",
                ToDate = "2026-01-20",
                Info = "Test"
            };
            response = await _client.PostAsJsonAsync("/api/sickleaves/add", sickLeave);
            var returnedSickLeave = await response.Content.ReadFromJsonAsync<SickLeave>();
           
           
            //Editing of SickLeave
            var editedSickLeave = new
            {
                Id = returnedSickLeave.Id,
                EmployeeId = returnedEmployee.Id,
                FromDate = "2026-01-18",
                ToDate = "2026-01-20",
                Info = "Edited info"
            };
            response = await _client.PutAsJsonAsync("/api/sickleaves/edit", editedSickLeave);
            var returnedEditedSickLeave = await response.Content.ReadFromJsonAsync<SickLeave>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Edited info", returnedEditedSickLeave.Info);

        }
    }
}
