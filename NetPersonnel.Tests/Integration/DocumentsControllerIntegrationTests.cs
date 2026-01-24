using Microsoft.AspNetCore.Mvc;
using NetPersonnel.DTOs;
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
    public class DocumentsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        public DocumentsControllerIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }


        [Fact]
        public async Task UploadDocument_AsHR_ReturnsOk()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "HR");
           

            var file = MockFile.CreateMockFile();

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "File", file.FileName); // "File" must match your DTO property
            content.Add(new StringContent("test.pdf"), "Filename");
            content.Add(new StringContent("1"), "DocumentTypeId");
            //content.Add(new StringContent("1"), "EmployeeId");
            var response = await _client.PostAsync("/api/documents/upload", content);
            var con = await response.Content.ReadAsStringAsync();
            _output.WriteLine(con);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact]
        public async Task DownloadDocument_AsHR_ReturnsFiles()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "HR");

            var file = MockFile.CreateMockFile();

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "File", file.FileName); // "File" must match your DTO property
            content.Add(new StringContent("test.pdf"), "Filename");
            content.Add(new StringContent("1"), "DocumentTypeId");
            //content.Add(new StringContent("1"), "EmployeeId");
            
            var response = await _client.PostAsync("/api/documents/upload", content);
            var returnedDoc = await response.Content.ReadFromJsonAsync<Document>();


            response = await _client.GetAsync($"/api/documents/download?id={returnedDoc.Id}");          
            var contentType = response.Content.Headers.ContentType?.MediaType;
            var contentDisposition = response.Content.Headers.ContentDisposition;
           
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(returnedDoc.ContentType, contentType);
            Assert.Equal(returnedDoc.Filename, contentDisposition?.FileName?.Trim('"'));


        }

        [Fact]
        public async Task DeleteDocument_AsHR_ReturnsNoContent()
        {
            _client.DefaultRequestHeaders.Add("Test-Role", "HR");


            var file = MockFile.CreateMockFile();

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "File", file.FileName); // "File" must match your DTO property
            content.Add(new StringContent("test.pdf"), "Filename");
            content.Add(new StringContent("1"), "DocumentTypeId");


            var response = await _client.PostAsync("/api/documents/upload", content);
            var returnedDoc = await response.Content.ReadFromJsonAsync<Document>();


            response = await _client.DeleteAsync($"/api/documents/delete?id={returnedDoc.Id}");
            
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
