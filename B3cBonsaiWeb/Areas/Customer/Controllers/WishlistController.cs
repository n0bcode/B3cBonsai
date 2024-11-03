using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    //Quản lý chức năng liên quan đến danh sách sản phẩm yêu thích của người dùng
    public class WishlistController : Controller
    {
        public IActionResult Index()
        {
            Random rd = new Random();
            var listObjs = rd.NextDouble() < 0.5 ? new List<Object>() : null;
            return View(listObjs);
        }
    }
}
