using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    //Quản lý các chức năng liên quan đến thông tin người dùng
    public class ClientProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult ProAddress()
        {
            return View();
        }
        public IActionResult ProWishlist()
        {
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        public IActionResult ProTickets()
        {
            return View();
        }
    }
}
