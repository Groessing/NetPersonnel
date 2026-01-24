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
    public class DepartmentsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public DepartmentsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AddDepartment_AsAdmin_ReturnsOk()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "Admin");
            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Body: {content}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task AddDepartment_AsUnauthorizedUser_ReturnsForbidden()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "");
            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }



        [Fact]
        public async Task EditDepartment_AsAdmin_ReturnsOk()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "Admin");
            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);
            var returnedDept = await response.Content.ReadFromJsonAsync<Department>();

            var editedDepartment = new
            {
                Id = returnedDept.Id,
                Name = "Test Department Edited"
            };

            response = await _client.PutAsJsonAsync("/api/departments/edit", editedDepartment);
            returnedDept = await response.Content.ReadFromJsonAsync<Department>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Test Department Edited", returnedDept.Name);
        }


        [Fact]
        public async Task EditDepartment_AsUnauthorizedUser_ReturnsForbidden()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "Admin");
            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);
            var returnedDept = await response.Content.ReadFromJsonAsync<Department>();
            
            _client.DefaultRequestHeaders.Remove("Test-Role");
            _client.DefaultRequestHeaders.Add("Test-Role", "");
            
            var editedDepartment = new
            {
                Id = returnedDept.Id,
                Name = "Test Department Edited"
            };

            response = await _client.PutAsJsonAsync("/api/departments/edit", editedDepartment);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }


        [Fact]
        public async Task DeleteDepartment_AsAdmin_ReturnsOk()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "Admin");
            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);
            var returnedDept = await response.Content.ReadFromJsonAsync<Department>();


            response = await _client.DeleteAsync($"/api/departments/delete?id={returnedDept.Id}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }


        [Fact]
        public async Task DeleteDepartment_AsUnauthorizedUser_ReturnsForbidden()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "Admin");
            var department = new
            {
                Name = "Test Department"
            };

            var response = await _client.PostAsJsonAsync("/api/departments/add", department);
            var returnedDept = await response.Content.ReadFromJsonAsync<Department>();
       
            _client.DefaultRequestHeaders.Remove("Test-Role");
            _client.DefaultRequestHeaders.Add("Test-Role", "");

            response = await _client.DeleteAsync($"/api/departments/delete?id={returnedDept.Id}");
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
