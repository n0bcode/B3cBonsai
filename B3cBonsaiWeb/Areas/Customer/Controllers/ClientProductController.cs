using Microsoft.AspNetCore.Mvc;
using B3cBonsai.Models;
using Microsoft.EntityFrameworkCore;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ClientProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientProductController(IUnitOfWork context)
        {
            _unitOfWork = context;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.SanPham.GetAll(includeProperties: "DanhMuc,HinhAnhs", filter: x => x.TrangThai);

            return View(products);
        }

        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _unitOfWork.SanPham.Get(includeProperties: "DanhMuc,HinhAnhs,BinhLuans,DanhSachYeuThichs", filter: x => x.TrangThai && x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
