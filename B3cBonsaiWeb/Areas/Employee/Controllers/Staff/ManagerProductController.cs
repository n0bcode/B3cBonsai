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
using B3cBonsai.DataAccess.Repository.IRepository;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageStorageService _imageStorageService;

        public ManagerProductController(IUnitOfWork unitOfWork, IImageStorageService imageStorageService)
        {
            _unitOfWork = unitOfWork;
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

            var product = await _unitOfWork.SanPham.Get(x => x.Id == id);
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
                    _unitOfWork.SanPham.Add(model.SanPham);
                    _unitOfWork.Save();

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
                            _unitOfWork.HinhAnhSanPham.Add(image);
                        }
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    var existingProduct = await _unitOfWork.SanPham.Get(x => x.Id == model.SanPham.Id);
                    if (existingProduct != null)
                    {
                        // Cập nhật thông tin sản phẩm
                        existingProduct.TenSanPham = model.SanPham.TenSanPham;
                        existingProduct.DanhMucId = model.SanPham.DanhMucId;
                        existingProduct.SoLuong = model.SanPham.SoLuong;
                        existingProduct.Gia = model.SanPham.Gia;
                        existingProduct.MoTa = model.SanPham.MoTa;

                        // Cập nhật sản phẩm vào cơ sở dữ liệu
                        _unitOfWork.SanPham.Update(existingProduct);
                        _unitOfWork.Save();


                        // Nếu người dùng không thêm hình ảnh mới, không cần xóa hình ảnh cũ
                        if (HinhAnhs != null && HinhAnhs.Any())
                        {
                            // Lấy danh sách hình ảnh cũ của sản phẩm
                            var existingImages = await _unitOfWork.HinhAnhSanPham.GetAll(
                                x => x.SanPhamId == model.SanPham.Id
                            );

                            // Xóa những hình ảnh cũ (xóa cả trong cơ sở dữ liệu và file system)
                            foreach (var image in existingImages)
                            {
                                await _imageStorageService.DeleteImageAsync(image.LinkAnh);
                                _unitOfWork.HinhAnhSanPham.Remove(image); // Xóa hình ảnh khỏi cơ sở dữ liệu
                            }
                            _unitOfWork.Save();

                            // Xử lý hình ảnh mới mà người dùng đã tải lên
                            var newImageUrls = await _imageStorageService.StoreImagesAsync(HinhAnhs, "product");
                            foreach (var imageUrl in newImageUrls)
                            {
                                var newImage = new HinhAnhSanPham
                                {
                                    SanPhamId = model.SanPham.Id,
                                    LinkAnh = imageUrl
                                };
                                _unitOfWork.HinhAnhSanPham.Add(newImage); // Thêm hình ảnh mới vào cơ sở dữ liệu
                            }
                            _unitOfWork.Save();

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
            SanPham product = await _unitOfWork.SanPham.Get(
                x => x.Id == id,
                "HinhAnhs,DanhMuc,Videos"
            );

                if (product == null)
                {
                    return NotFound();
                }

            return PartialView(product);
        }

        #region//GET API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _unitOfWork.SanPham.GetAll(
                null,
                "HinhAnhs,DanhMuc"
            );

            var result = products.Select(sp => new
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

            return Json(new { data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.SanPham.Get(x => x.Id == id);
            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });

            try
            {
                var binhLuans = await _unitOfWork.BinhLuan.GetAll(x => x.SanPhamId == id);
                _unitOfWork.BinhLuan.RemoveRange(binhLuans);

                var danhSachYeuThichs = await _unitOfWork.DanhSachYeuThich.GetAll(x => x.SanPhamId == id);
                _unitOfWork.DanhSachYeuThich.RemoveRange(danhSachYeuThichs);

                var hinhAnhs = await _unitOfWork.HinhAnhSanPham.GetAll(x => x.SanPhamId == id);
                _unitOfWork.HinhAnhSanPham.RemoveRange(hinhAnhs);

                var videoSanPhams = await _unitOfWork.VideoSanPham.GetAll(x => x.SanPhamId == id);
                _unitOfWork.VideoSanPham.RemoveRange(videoSanPhams);

                _unitOfWork.Save();
                _unitOfWork.SanPham.Remove(product);
                _unitOfWork.Save();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bạn không thể xóa sản phẩm này." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var product = await _unitOfWork.SanPham.Get(x => x.Id == id);
            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });

            try
            {
                product.TrangThai = !product.TrangThai; // Mark product as inactive
                _unitOfWork.SanPham.Update(product);
                _unitOfWork.Save();
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
            return (await _unitOfWork.DanhMucSanPham.GetAll())
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.TenDanhMuc
                })
                .ToList();
        }
        #endregion
        public async Task<IActionResult> ExportProductsToExcel()
        {
            var products = (await _unitOfWork.SanPham.GetAll(null, "DanhMuc")).ToList(); // Lấy danh sách sản phẩm từ cơ sở dữ liệu

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
                    worksheet.Cell(currentRow, 3).Value = product.DanhMuc.TenDanhMuc; // Hoặc product.DanhMuc.TenDanhMuc nếu bạn muốn tên danh mục
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