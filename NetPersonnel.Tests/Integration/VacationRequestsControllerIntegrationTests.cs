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
    public class VacationRequestsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        public VacationRequestsControllerIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task AddVacationRequest_AsEmployee_ReturnsOk()
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
            _client.DefaultRequestHeaders.Add("Test-Role", "Employee");
            
            //Creation of Vacation Request
            var vacationRequest = new
            {
                FromDate = "2026-01-19",
                ToDate = "2026-01-20",
            };
            response = await _client.PostAsJsonAsync("/api/vacationrequests/add", vacationRequest);
            var returnedVacationRequest = await response.Content.ReadFromJsonAsync<VacationRequest>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }


        [Fact]
        public async Task EditVacationRequest_AsHR_ReturnsOk()
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
            _client.DefaultRequestHeaders.Add("Test-Role", "Employee");

            //Creation of Sick Leave
            var vacationRequest = new
            {
                FromDate = "2026-01-19",
                ToDate = "2026-01-20",
            };
            response = await _client.PostAsJsonAsync("/api/vacationrequests/add", vacationRequest);
            var returnedVacationRequest = await response.Content.ReadFromJsonAsync<VacationRequest>();


            _client.DefaultRequestHeaders.Remove("Test-Role");
            _client.DefaultRequestHeaders.Add("Test-Role", "HR");

            response = await _client.PatchAsync($"/api/vacationrequests/edit?requestId={returnedVacationRequest.Id}&statusId={2}", null);
            var returnedEditedVacReq = await response.Content.ReadFromJsonAsync<VacationRequest>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, returnedEditedVacReq.StatusId);

        }
    }
}
