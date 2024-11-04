using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Admin
{
    [Area("Employee")]
    public class ManagerCategoryController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
