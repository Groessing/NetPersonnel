using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPersonnel.Tests.Service
{
    public class MockFile
    {
        public static IFormFile CreateMockFile()
        {
            string fileName = "test.pdf";
            string content = "Fake file content";
            string contentType = "application/pdf";

            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, bytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }
}
