using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.ViewComponents
{
    //Hiển thị danh sách sản phẩm trong /Customer/Cart/Index
    //Hiển thị danh sách sản phẩm trong /Customer/Home/Index
    //Hiển thị danh sách sản phẩm được thịnh hành (yêu thích/đặt hàng nhiều) - 9 sản phẩm hiển thị
    public class PopularProductsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Random rd = new Random();
            var listObjs = rd.NextDouble() < 0.5 ? new List<Object>() : null;
            return View(listObjs);
        }
    }
}
