using System.Linq;
using System.Threading.Tasks;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.Models;
using B3cBonsai.Models.ViewModels;
using B3cBonsai.Utility.Extentions;
using B3cBonsai.Utility.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageStorageService _imageStorageService;

        public ManagerProductController(ApplicationDbContext context, IImageStorageService imageStorageService)
        {
            _context = context;
            _imageStorageService = imageStorageService;
        }

        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Staff}")]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            SanPhamVM sanPhamVM = new SanPhamVM();
            sanPhamVM.DanhMucSanPham = await DanhMucSanPham();

            if (!id.HasValue)
            {
                sanPhamVM.SanPham = new SanPham();
                return PartialView(sanPhamVM);
            }

            var product = await _context.SanPhams.FindAsync(id);
            if (product == null)
            {
                return PartialView(sanPhamVM);
            }
            sanPhamVM.SanPham = product;

            return PartialView(sanPhamVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(SanPhamVM model, IFormFile[] HinhAnhs)
        {
            string viewHtml = null;
            if (ModelState.IsValid)
            {
                if (model.SanPham.Id == 0)
                {
                    model.SanPham.TrangThai = true;
                    _context.SanPhams.Add(model.SanPham);
                    await _context.SaveChangesAsync();

                    if (HinhAnhs != null && HinhAnhs.Any())
                    {
                        var imageUrls = await _imageStorageService.StoreImagesAsync(HinhAnhs, "product");
                        foreach (var imageUrl in imageUrls)
                        {
                            var image = new HinhAnhSanPham
                            {
                                SanPhamId = model.SanPham.Id,
                                LinkAnh = imageUrl
                            };
                            _context.HinhAnhSanPhams.Add(image);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    var existingProduct = await _context.SanPhams.FindAsync(model.SanPham.Id);
                    if (existingProduct != null)
                    {
                        // Cập nhật thông tin sản phẩm
                        existingProduct.TenSanPham = model.SanPham.TenSanPham;
                        existingProduct.DanhMucId = model.SanPham.DanhMucId;
                        existingProduct.SoLuong = model.SanPham.SoLuong;
                        existingProduct.Gia = model.SanPham.Gia;
                        existingProduct.MoTa = model.SanPham.MoTa;

                        // Cập nhật sản phẩm vào cơ sở dữ liệu
                        _context.SanPhams.Update(existingProduct);
                        await _context.SaveChangesAsync();


                        // Nếu người dùng không thêm hình ảnh mới, không cần xóa hình ảnh cũ
                        if (HinhAnhs != null && HinhAnhs.Any())
                        {
                            // Lấy danh sách hình ảnh cũ của sản phẩm
                            var existingImages = await _context.HinhAnhSanPhams
                                .Where(x => x.SanPhamId == model.SanPham.Id)
                                .ToListAsync();

                            // Xóa những hình ảnh cũ (xóa cả trong cơ sở dữ liệu và file system)
                            foreach (var image in existingImages)
                            {
                                await _imageStorageService.DeleteImageAsync(image.LinkAnh);
                                _context.HinhAnhSanPhams.Remove(image); // Xóa hình ảnh khỏi cơ sở dữ liệu
                            }
                            await _context.SaveChangesAsync();

                            // Xử lý hình ảnh mới mà người dùng đã tải lên
                            var newImageUrls = await _imageStorageService.StoreImagesAsync(HinhAnhs, "product");
                            foreach (var imageUrl in newImageUrls)
                            {
                                var newImage = new HinhAnhSanPham
                                {
                                    SanPhamId = model.SanPham.Id,
                                    LinkAnh = imageUrl
                                };
                                _context.HinhAnhSanPhams.Add(newImage); // Thêm hình ảnh mới vào cơ sở dữ liệu
                            }
                            await _context.SaveChangesAsync();

                        }
                        else
                        {
                            // Nếu không có ảnh mới, giữ nguyên ảnh cũ mà không làm gì cả
                            // Không cần làm gì thêm trong trường hợp này, các hình ảnh cũ sẽ không bị xóa
                        }
                    }
                }

                return Json(new { success = true });
            }
            model.DanhMucSanPham = await DanhMucSanPham();
            viewHtml = await this.RenderViewAsync("Upsert", model, true);

            return Json(new { success = false, data = viewHtml });
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
                _context.RemoveRange(_context.BinhLuans.Where(x => x.SanPhamId == id));
                _context.RemoveRange(_context.DanhSachYeuThichs.Where(x => x.SanPhamId == id));
                _context.RemoveRange(_context.HinhAnhSanPhams.Where(x => x.SanPhamId == id));
                _context.RemoveRange(_context.VideoSanPhams.Where(x => x.SanPhamId == id));
                _context.SaveChanges();
                _context.SanPhams.Remove(product);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bạn không thể xóa sản phẩm này." });
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
                product.TrangThai = !product.TrangThai; // Mark product as inactive
                _context.SanPhams.Update(product);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }
        [NonAction]
        private async Task<List<SelectListItem>> DanhMucSanPham()
        {
            return await _context.DanhMucSanPhams
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.TenDanhMuc
                })
                .ToListAsync();
        }
        #endregion
        public IActionResult ExportProductsToExcel()
        {
            var products = _context.SanPhams.Include(p => p.DanhMuc).ToList(); // Lấy danh sách sản phẩm từ cơ sở dữ liệu

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachSanPham"); // Tạo worksheet mới
                var currentRow = 1;

                // Tiêu đề cho các cột
                worksheet.Cell(currentRow, 1).Value = "Mã sản phẩm";          // Id
                worksheet.Cell(currentRow, 2).Value = "Tên sản phẩm";        // TenSanPham
                worksheet.Cell(currentRow, 3).Value = "Danh mục";           // DanhMucId
                worksheet.Cell(currentRow, 4).Value = "Mô tả";               // MoTa
                worksheet.Cell(currentRow, 5).Value = "Số lượng";            // SoLuong
                worksheet.Cell(currentRow, 6).Value = "Giá";                 // Gia
                worksheet.Cell(currentRow, 7).Value = "Ngày tạo";            // NgayTao
                worksheet.Cell(currentRow, 8).Value = "Ngày sửa đổi";        // NgaySuaDoi
                worksheet.Cell(currentRow, 9).Value = "Trạng thái";          // TrangThai

                // Nội dung
                foreach (var product in products)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = product.Id;
                    worksheet.Cell(currentRow, 2).Value = product.TenSanPham;
                    worksheet.Cell(currentRow, 3).Value = product.DanhMucId; // Hoặc product.DanhMuc.TenDanhMuc nếu bạn muốn tên danh mục
                    worksheet.Cell(currentRow, 4).Value = product.MoTa;
                    worksheet.Cell(currentRow, 5).Value = product.SoLuong;
                    worksheet.Cell(currentRow, 6).Value = product.Gia;
                    worksheet.Cell(currentRow, 7).Value = product.NgayTao.ToString("dd/MM/yyyy"); // Định dạng ngày
                    worksheet.Cell(currentRow, 8).Value = product.NgaySuaDoi.ToString("dd/MM/yyyy"); // Định dạng ngày
                    worksheet.Cell(currentRow, 9).Value = product.TrangThai ? "Còn hàng" : "Hết hàng"; // Hiển thị trạng thái
                }

                worksheet.Columns().AdjustToContents(); // Điều chỉnh độ rộng cột tự động

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachSanPham.xlsx");
                }
            }
        }

    }
}
