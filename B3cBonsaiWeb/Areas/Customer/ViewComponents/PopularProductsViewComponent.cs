using B3cBonsai.DataAccess.Repository;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.ViewComponents
{
    //Hiển thị danh sách sản phẩm trong /Customer/Cart/Index
    //Hiển thị danh sách sản phẩm trong /Customer/Home/Index
    //Hiển thị danh sách sản phẩm được thịnh hành (yêu thích/đặt hàng nhiều) - 9 sản phẩm hiển thị
    public class PopularProductsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unit;
        public PopularProductsViewComponent(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var chiTietDonHangs = await _unit.ChiTietDonHang.GetAll();
            var sanPhams = await _unit.SanPham.GetAll();

            var sanPhamBanChay = chiTietDonHangs
                .Where(ct => ct.SanPhamId != null)
                .GroupBy(ct => ct.SanPhamId)
                .Select(group => new
                {
                    SanPhamId = group.Key,
                    TongSoLuongBan = group.Sum(ct => ct.SoLuong)
                })
                .OrderByDescending(x => x.TongSoLuongBan)
                .Take(9)
                .Select(x => x.SanPhamId)
                .ToList();

            IEnumerable<SanPham>? sps = (await _unit.SanPham.GetAll(includeProperties: "HinhAnhs,DanhMuc", filter: sp => sanPhamBanChay.Contains(sp.Id))).ToList();
            return View(sps);
        }
    }
}
