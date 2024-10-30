using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Admin
{
    public class ManagerCustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
