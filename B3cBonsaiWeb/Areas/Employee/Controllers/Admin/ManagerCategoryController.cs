using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Admin
{
    public class ManagerCategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
