using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers
{
    public class EmployeeProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
