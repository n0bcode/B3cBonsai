using Microsoft.AspNetCore.Mvc;
using B3cBonsai.Models;
using Microsoft.EntityFrameworkCore;
using B3cBonsai.DataAccess.Data;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ClientProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _context.SanPhams
                .Include(sp => sp.DanhMuc)  // Bao gồm thông tin danh mục
                .Include(sp => sp.HinhAnhs) // Bao gồm thông tin hình ảnh
                .Where(sp => sp.TrangThai)  // Lọc sản phẩm đang hoạt động
                .ToListAsync();

            return View(products);
        }

        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.SanPhams
                .Include(sp => sp.DanhMuc)
                .Include(sp => sp.HinhAnhs)
                .Include(sp => sp.BinhLuans)  // Bao gồm thông tin bình luận
                .Include(sp => sp.DanhSachYeuThichs) // Bao gồm danh sách yêu thích
                .FirstOrDefaultAsync(sp => sp.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
