using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.ViewComponents
{
    public class CommonLayoutViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unit;
        public CommonLayoutViewComponent(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách sản phẩm ngẫu nhiên
            var sps = (await _unit.SanPham.GetAll(includeProperties: "HinhAnhs,DanhMuc"))
                        .OrderBy(x => x.Id)
                        .Skip(9)
                        .Take(9)
                        .ToList();

            // Trả về view với mô hình là danh sách sản phẩm
            return View(sps); // Đảm bảo rằng view 'Default' tồn tại
        }
    }
}
