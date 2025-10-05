using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace B3cBonsaiWeb.Areas.Customer.ViewComponents
{
    public class LeftBarFilterViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unit;
        public LeftBarFilterViewComponent(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var filterValueProduct = new FilterValueVM();
            var sanPhams = await _unit.SanPham.GetAll(x => x.TrangThai);
            filterValueProduct.Categories = (await _unit.DanhMucSanPham.GetAll(null, "SanPhams")).Select(X => new DanhMucSanPham
            {
                Id = X.Id,
                TenDanhMuc = X.TenDanhMuc,
                SanPhams = sanPhams.Where(sp => sp.DanhMucId == X.Id).ToList() 
            }).ToList();

            filterValueProduct.MinimumPrice = Convert.ToInt32(sanPhams.Min(x => x.Gia));
            filterValueProduct.MaximumPrice = Convert.ToInt32(sanPhams.Max(x => x.Gia));
            filterValueProduct.InStock = sanPhams.Count(x => x.SoLuong > 0); // Kiểm tra số sản phẩm còn hàng
            filterValueProduct.OutOfStock = sanPhams.Count(x => x.SoLuong <= 0); // Kiểm tra số sản phẩm hết hàng

            return View(filterValueProduct);
        }
    }
}