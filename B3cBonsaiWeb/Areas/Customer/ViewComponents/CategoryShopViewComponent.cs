using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.ViewComponents
{
    //Hiển thị danh sách sản phẩm trong /Customer/Cart/Index khi giỏ hàng trống
    public class CategoryShopViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Random rd = new Random();
            var listObjs = rd.NextDouble() < 0.5 ? new List<Object>() : null;
            return View(listObjs);
        }
    }
}
