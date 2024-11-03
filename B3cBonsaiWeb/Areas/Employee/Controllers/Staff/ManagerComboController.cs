using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerComboController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
