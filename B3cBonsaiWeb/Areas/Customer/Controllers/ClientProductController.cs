using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    //Quản lý chức năng hiển thị view sản phẩm cho người dùng
    public class ClientProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail()
        {
            return View();
        }
    }
}
