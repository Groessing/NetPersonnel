using NetPersonnel.Models;
using NetPersonnel.Tests.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using System.Net.Http.Json;

namespace NetPersonnel.Tests.Integration
{
    public class UsersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        public UsersControllerIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;

        }
        [Fact]
        public async Task SetInactive_AsAdmin_ReturnsOk()
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

         
            //Creation of User
            var user = new
            {
                Username = "test",
                EmployeeId = returnedEmployee.Id,
                Password = "Test",
                RoleId = 1,
                IsActive = true
            };

            response = await _client.PostAsJsonAsync("/api/users/add", user);
            var content = await response.Content.ReadAsStringAsync();
            //_output.WriteLine(content);
            var returnedUser = await response.Content.ReadFromJsonAsync<User>();
           
            response = await _client.PatchAsync($"/api/users/edit?userId={returnedUser.Id}&isActive={false}", null);
            var returnedEditedUser = await response.Content.ReadFromJsonAsync<User>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(returnedEditedUser.IsActive);

        }

    }
}
