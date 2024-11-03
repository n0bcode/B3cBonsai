using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Admin
{
    [Area("Employee")]
    public class ManagerEmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert()
        {
            return View();
        }
        public IActionResult DetailWithDelete()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
    }
}
