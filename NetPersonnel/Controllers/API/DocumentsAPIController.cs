using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetPersonnel.Data;
using NetPersonnel.DTOs;
using NetPersonnel.Models;
using NetPersonnel.Services;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace NetPersonnel.Controllers.API
{
    [Authorize]
    [ApiController]
    [Route("api/documents")]
    public class DocumentsAPIController : ControllerBase
    {
        private ApplicationDBContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IAppLogger _logger;

        //This is used to store the certain Content-Type in DB
        public readonly Dictionary<string, string> Extensions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { ".pdf", "application/pdf" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".ppt", "application/vnd.ms-powerpoint" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { ".txt", "text/plain" },
            { ".csv", "text/csv" }
        };
        public DocumentsAPIController(ApplicationDBContext db, IWebHostEnvironment env, IAppLogger logger)
        {
            _db = db;
            _env = env;
            _logger = logger;
        }


        //GET: api/documents/get
        //Returns documents depending on the user's role
        [HttpGet("get")]
        public IActionResult GetDocuments()
        {
            if (User.IsInRole("HR"))
                return Ok(GetAllDocuments());

            else if (User.IsInRole("Manager"))
                return Ok(GetDocumentsByDepartment());

            else if (User.IsInRole("Employee"))
                return Ok(GetDocumentsByEmployee());

            else return BadRequest();
        }


        //Returns all documents with related data
        //Used only by HR
        public IActionResult GetAllDocuments()
        {
            var documents = _db.Documents.AsNoTracking()                //Read-only query
                                         .Include(d => d.Employee)      //Include employee info
                                         .Include(d => d.User)          //Include uploader info
                                         .Include(d => d.DocumentType)  //Include document type
                                         .ToList();
            return Ok(documents);
        }


        //Returns documents belonging to employees from the manager's department
        public IActionResult GetDocumentsByDepartment()
        {
            var deptId = int.Parse(User.FindFirst("DepartmentID").Value);
            var documents = _db.Documents.Where(d => d.Employee.DepartmentId == deptId)
                                         .Include(d => d.Employee)
                                         .Include(d => d.DocumentType)
                                         .ToList();

            return Ok(documents);
        }


        //Returns documents for the current employee
        //Includes documents that are public (EmployeeId == null)
        public IActionResult GetDocumentsByEmployee()
        {

            var employeeIdClaim = User.FindFirst("EmployeeID");
            if (employeeIdClaim == null)
                return BadRequest("EmployeeID claim missing.");

            var employeeId = int.Parse(employeeIdClaim.Value);
            var documents = _db.Documents.Where(d => d.EmployeeId == employeeId || d.EmployeeId == null)    //Employee gets documents with EmployeeID = ID or EmployeeID = null (null = viewable by everyone)
                                        .Include(d => d.Employee)
                                        .Include(d => d.DocumentType).ToList();

            return Ok(documents);
        }

        //POST: api/documents/upload
        //Uploads a document and stores metadata in the database
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentDTO dto)
        {
            //Authorization Check
            if (!User.IsInRole("HR") && !User.IsInRole("Employee"))
                return Forbid();


            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("File is missing");
            


            string folderName = "";

            //If Employee uploads, document is linked to their EmployeeID
            if (User.IsInRole("Employee"))
            {
                dto.EmployeeId = int.Parse(User.FindFirst("EmployeeID").Value);

                folderName = User.FindFirst(ClaimTypes.Name).Value;
            }

            //HR uploads go into the HR folder
            else if (User.IsInRole("HR"))
            {
                folderName = "HR";
            }

            //UserID is stored in claims
            int uploadedBy = int.Parse(User.FindFirst("UserID").Value);

            
            UploadToDisk(dto, folderName);

            //Determine MIME type based on file extension
            var extension = Path.GetExtension(dto.File.FileName);

            //New document will be created
            var document = new Models.Document
            {
                Filename = dto.Filename,
                FilePath = $"Documents/{folderName}",
                EmployeeId = dto.EmployeeId,
                UploadedBy = uploadedBy,
                UploadedAt = DateTime.Now,
                DocumentTypeId = dto.DocumentTypeId,
                ContentType = Extensions[extension]
            };
            _db.Documents.Add(document);
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "uploaded a document", document.Id, ip, "");

            return Ok(document);
        }





        //GET: api/documents/download?id=5
        //Downloads a document if the user is authorized
        [HttpGet("download")]
        public async Task<IActionResult> DownloadDocument([FromQuery] int id)
        {            
            var document = await _db.Documents.FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
                return NotFound();


            if (User.IsInRole("Employee"))
            {
                int employeeId = int.Parse(User.FindFirst("EmployeeID").Value);



                //Employee cannot download documents belonging to other employees
                if (employeeId != document.EmployeeId)
                    return Forbid();
            }

            //Manager authorization check
            else if (User.IsInRole("Manager"))
            {
                int? departmentId = int.Parse(User.FindFirst("DepartmentID").Value);

                int? empDeptId = await _db.Employees.Where(e => e.Id == document.EmployeeId)
                                                    .Select(e => e.DepartmentId)
                                                    .FirstOrDefaultAsync();

                //Manager cannot download documents from another department
                if (departmentId != empDeptId) 
                   return Forbid();
            }




           var fullPath = Path.Combine(_env.ContentRootPath, document.FilePath, document.Filename);


           if (!System.IO.File.Exists(fullPath))
               return NotFound("File not found on disk");


           var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
           var contentType = document.ContentType ?? "application/octet-stream";

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "downloaded a document", document.Id, ip, "");


            return File(fileBytes, contentType, document.Filename);

        }



        // DELETE: api/documents/delete?id=5
        // Deletes a document from disk and database
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteDocument([FromQuery] int id)
        {
            var document = await _db.Documents.FindAsync(id);
            if(document == null)
                return NotFound();
           
            string folderName = "";

            //Employee can only delete documents they uploaded
            if (User.IsInRole("Employee"))
            {
                int userId = int.Parse(User.FindFirst("UserID").Value);
                if (document.UploadedBy != userId)
                    return Forbid();

                folderName = User.FindFirst(ClaimTypes.Name).Value;
            }

            //HR can delete HR documents
            else if (User.IsInRole("HR"))
                folderName = "HR";


            DeleteFromDisk(document);   

            _db.Documents.Attach(document);
            _db.Documents.Remove(document);
            await _db.SaveChangesAsync();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logger.LogAsync(int.Parse(User.FindFirst("UserID").Value), "deleted a document", document.Id, ip, "");


            return NoContent();
        }




        //Saves uploaded file to disk
        private async void UploadToDisk(DocumentDTO dto, string folderName)
        {
            var uploadPath = Path.Combine(_env.ContentRootPath, "Documents", folderName);
            Directory.CreateDirectory(uploadPath);

            var fileName = $"{Path.GetFileName(dto.Filename)}";
            var filePath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.File.CopyToAsync(stream);
        }


        //Deletes file from disk
        private void DeleteFromDisk(Models.Document document)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, document.FilePath, document.Filename);

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}
