using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Admin
{
    [Area("Employee")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
