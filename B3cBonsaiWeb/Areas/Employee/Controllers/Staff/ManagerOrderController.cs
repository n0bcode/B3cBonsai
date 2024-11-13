using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ManagerOrderController(ApplicationDbContext unitOfWork)
        {
			_context = unitOfWork;
        }
        // GET: Employee/ManagerOrder
        [Authorize]
        public IActionResult Index(string searchTerm)
        {
            var ordersQuery = _context.DonHangs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                ordersQuery = ordersQuery.Where(dh => dh.TenNguoiNhan.Contains(searchTerm) ||
                                                      dh.SoTheoDoi.Contains(searchTerm) ||
                                                      dh.TrangThaiDonHang.Contains(searchTerm));
            }

            var orders = ordersQuery
                .Select(dh => new OrderViewModel
                {
                    Id = dh.Id,
                    TenNguoiNhan = dh.TenNguoiNhan,
                    SoTheoDoi = dh.SoTheoDoi,
                    TrangThaiDonHang = dh.TrangThaiDonHang,
                    NgayDatHang = dh.NgayDatHang,
                    NgayNhanHang = dh.NgayNhanHang,
                    TrangThaiThanhToan = dh.TrangThaiThanhToan,
                    TongTienDonHang = dh.TongTienDonHang,
                    ChiTietDonHangs = dh.ChiTietDonHangs.Select(ctdh => new OrderItemViewModel
                    {
                        Id = ctdh.Id,
                        SoLuong = ctdh.SoLuong,
                        SanPham = new ProductViewModel
                        {
                            TenSanPham = ctdh.SanPham.TenSanPham,
                            HinhAnhs = ctdh.SanPham.HinhAnhs.Select(ha => new ProductImageViewModel { LinkAnh = ha.LinkAnh }).ToList()
                        }
                    }).ToList()
                }).ToList();

            return View(orders);
        }

        public IActionResult OrderSummary()
		{

			return View();
		}


	}
}
	


