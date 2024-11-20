using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.ViewComponents
{
    //Hiển thị danh sách sản phẩm trong /Customer/Cart/Index khi giỏ hàng trống
    public class CategoryShopViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unit;
        public CategoryShopViewComponent(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<SanPham> sps = (await _unit.SanPham.GetAll(includeProperties:"HinhAnhs")).OrderBy(x => x.Id).Take(9).ToList();
            return View(sps);
        }
    }
}
