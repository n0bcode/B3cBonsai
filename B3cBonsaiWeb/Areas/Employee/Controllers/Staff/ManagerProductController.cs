using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.Models;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null)
            {
                return PartialView(new SanPham());
            }
            var product = await _context.SanPhams.FindAsync(id);

            if (product == null) {
                return PartialView(new SanPham());
            } 
            return View(product);
        }

        [HttpPost]
        public IActionResult Upsert([FromBody] SanPham model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                if (model.Id == 0)
                {
                    _context.SanPhams.Add(model);
                }
                else
                {
                    _context.SanPhams.Update(model);
                }
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        public IActionResult DetailWithDelete()
        {
            return PartialView("_DetailWithDelete");
        }

        [HttpGet]
        public async Task<IActionResult> DetailWithDelete(int id)
        {

            if (id == null || id <= 0)
            {
                return Json(new { error = "Id không hợp lệ" });
            }

            var product = await _context.SanPhams
                        .Include(a => a.HinhAnhs)
                        .Include(a => a.DanhMuc)
                        .Include(a => a.Videos)
                        .Select(a => new
                        {
                            a.Id,
                            a.TenSanPham,
                            a.Gia,
                            a.MoTa,
                            a.DanhMuc.TenDanhMuc,
                            a.SoLuong,
                            a.NgayTao,
                            a.NgaySuaDoi,
                            a.TrangThai,
                            Videos = a.Videos.Select(v => new { v.LinkVideo }).ToList(),
                            HinhAnhs = a.HinhAnhs.Select(ha => new { ha.LinkAnh }).ToList()

                        })
                        .FirstOrDefaultAsync(a => a.Id == id);

            if (product == null)
            {
                return Json(new { error = "Không tìm thấy sản phẩm" });
            }

            return Json(new { data = product });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.SanPhams
                .Where(a => a.TrangThai == true)
                .Include(sp => sp.HinhAnhs)
                .Include(sp => sp.DanhMuc)
                .Select(sp => new
                {
                    sp.Id,
                    sp.TenSanPham,
                    sp.DanhMuc.TenDanhMuc,
                    sp.SoLuong,
                    sp.Gia,
                    sp.NgayTao,
                    sp.NgaySuaDoi,
                    sp.TrangThai,
                    HinhAnhs = sp.HinhAnhs.Select(ha => new { ha.LinkAnh }).ToList()
                })
                .ToList();

            return Json(new { data = products });
        }



        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _context.SanPhams.Find(id);
            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });

            try
            {
                product.TrangThai = false;
                _context.SanPhams.Update(product);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }
    }

}
