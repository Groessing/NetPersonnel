using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NetPersonnel.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult VacationRequests()
        {
            return View();
        }

        public IActionResult SickLeaves()
        {
            return View();
        }

        public IActionResult Documents()
        {
            return View();
        }
    }
}
