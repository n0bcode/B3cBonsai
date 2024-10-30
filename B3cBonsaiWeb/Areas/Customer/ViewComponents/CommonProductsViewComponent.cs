using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.ViewComponents
{
    //Hiển thị danh sách sản phẩm trong /Customer/Home/Index
    //Hiển thị danh sách 1 số sản phẩm ngẫu nhiên - 9 sản phẩm hiển thị
    public class CommonProductsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Random rd = new Random();
            var listObjs = rd.NextDouble() < 0.5 ? new List<Object>() : null;
            return View(listObjs);
        }
    }
}
