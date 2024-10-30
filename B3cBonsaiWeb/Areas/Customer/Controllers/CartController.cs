using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    //Quản lý tính năng liên quan đến giỏ hàng
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            Random rd = new Random();
            var listObjs = rd.NextDouble() < 0.5 ? new List<Object>() : null;
            return View(listObjs);
        }
    }
}
