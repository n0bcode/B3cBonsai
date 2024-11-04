using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerComboController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
