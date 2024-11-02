using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    // Quản lý tính năng truy cập của người dùng
    public class AccountController : Controller
    {
        public IActionResult Login(string? typeUser)
        {
            if (typeUser == null || typeUser == "user")
            {
                return View("UserLogin");
            }
            return View("EmployeeLogin");
        }
    }
}

