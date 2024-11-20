using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.ViewComponents
{
    //Hiển thị danh sách sản phẩm trong /Customer/Home/Index
    //Hiển thị danh sách 1 số sản phẩm ngẫu nhiên - 9 sản phẩm hiển thị
    public class CommonProductsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unit;
        public CommonProductsViewComponent(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<SanPham>? sps = (await _unit.SanPham.GetAll(includeProperties: "HinhAnhs,DanhMuc")).OrderBy(x => x.Id).Skip(9).Take(9).ToList();
            return View(sps);
        }
    }
}
