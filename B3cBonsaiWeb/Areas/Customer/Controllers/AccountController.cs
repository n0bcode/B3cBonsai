using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login(string? typeUser)
        {
            if (typeUser == null || typeUser == "user")
            {
                return View("UserLogin");
            }
            return View();
        }
    }
}
