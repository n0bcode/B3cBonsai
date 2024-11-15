using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ManagerProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            // Retrieve categories for dropdown list
            ViewBag.DanhMucOptions = await _context.DanhMucSanPhams
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.TenDanhMuc
                })
                .ToListAsync();

            if (!id.HasValue)
            {
                return PartialView(new SanPham());
            }

            var product = await _context.SanPhams.FindAsync(id);
            if (product == null)
            {
                return PartialView(new SanPham());
            }

            return PartialView(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(SanPham model, IFormFile[] HinhAnhs)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    model.TrangThai = true;
                    _context.SanPhams.Add(model);
                    await _context.SaveChangesAsync();

                    if (HinhAnhs != null && HinhAnhs.Any())
                    {
                        foreach (var file in HinhAnhs)
                        {
                            if (file.Length > 0)
                            {
                                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "product", file.FileName);
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var image = new HinhAnhSanPham
                                {
                                    SanPhamId = model.Id,
                                    LinkAnh = "/images/product/" + file.FileName
                                };
                                _context.HinhAnhSanPhams.Add(image);
                            }
                        }
                        await _context.SaveChangesAsync();
                        TempData["ThanhCong"] = "Sản phẩm đã được thêm thành công!";
                    }
                }
                else
                {
                    var existingProduct = await _context.SanPhams.FindAsync(model.Id);
                    if (existingProduct != null)
                    {
                        // Cập nhật thông tin sản phẩm
                        existingProduct.TenSanPham = model.TenSanPham;
                        existingProduct.DanhMucId = model.DanhMucId;
                        existingProduct.SoLuong = model.SoLuong;
                        existingProduct.Gia = model.Gia;
                        existingProduct.MoTa = model.MoTa;

                        // Cập nhật sản phẩm vào cơ sở dữ liệu
                        _context.SanPhams.Update(existingProduct);
                        await _context.SaveChangesAsync();

                        TempData["ThanhCong"] = "Sản phẩm đã được sửa thành công!";

                        // Nếu người dùng không thêm hình ảnh mới, không cần xóa hình ảnh cũ
                        if (HinhAnhs != null && HinhAnhs.Any())
                        {
                            // Lấy danh sách hình ảnh cũ của sản phẩm
                            var existingImages = await _context.HinhAnhSanPhams
                                .Where(x => x.SanPhamId == model.Id)
                                .ToListAsync();

                            // Xóa những hình ảnh cũ (xóa cả trong cơ sở dữ liệu và file system)
                            foreach (var image in existingImages)
                            {
                                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.LinkAnh.TrimStart('/'));
                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath); // Xóa hình ảnh khỏi file system
                                }
                                _context.HinhAnhSanPhams.Remove(image); // Xóa hình ảnh khỏi cơ sở dữ liệu
                            }
                            await _context.SaveChangesAsync();

                            // Xử lý hình ảnh mới mà người dùng đã tải lên
                            foreach (var file in HinhAnhs)
                            {
                                if (file.Length > 0)
                                {
                                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "product", file.FileName);
                                    using (var stream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }

                                    var newImage = new HinhAnhSanPham
                                    {
                                        SanPhamId = model.Id,
                                        LinkAnh = "/images/product/" + file.FileName
                                    };
                                    _context.HinhAnhSanPhams.Add(newImage); // Thêm hình ảnh mới vào cơ sở dữ liệu
                                }
                            }
                            await _context.SaveChangesAsync();

                            TempData["ThanhCong"] = "Sản phẩm đã được sửa thành công!";
                        }
                        else
                        {
                            // Nếu không có ảnh mới, giữ nguyên ảnh cũ mà không làm gì cả
                            // Không cần làm gì thêm trong trường hợp này, các hình ảnh cũ sẽ không bị xóa
                        }
                    }
                }


                return RedirectToAction("Index", "ManagerProduct");
            }
            return RedirectToAction("Index", "ManagerProduct");
        }



        [HttpGet]
        public async Task<IActionResult> DetailWithDelete(int id)
        {
            SanPham product = await _context.SanPhams
                .Include(a => a.HinhAnhs)
                .Include(a => a.DanhMuc)
                .Include(a => a.Videos)
                .Select(a => new SanPham
                {
                    Id = a.Id,
                    TenSanPham = a.TenSanPham,
                    Gia = a.Gia,
                    MoTa = a.MoTa,
                    DanhMuc = a.DanhMuc,
                    SoLuong = a.SoLuong,
                    NgayTao = a.NgayTao,
                    NgaySuaDoi = a.NgaySuaDoi,
                    TrangThai = a.TrangThai,
                    Videos = a.Videos.ToList(),
                    HinhAnhs = a.HinhAnhs.ToList()
                })
                .FirstOrDefaultAsync(a => a.Id == id);


            return PartialView(product);
        }

        #region//GET API
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.SanPhams
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
                product.TrangThai = false; // Mark product as inactive
                _context.SanPhams.Update(product);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }
        [HttpPost]
        public IActionResult ChangeStatus(int id)
        {
            var product = _context.SanPhams.Find(id);
            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });

            try
            {
                product.TrangThai = false; // Mark product as inactive
                _context.SanPhams.Update(product);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }
        #endregion
    }
}
