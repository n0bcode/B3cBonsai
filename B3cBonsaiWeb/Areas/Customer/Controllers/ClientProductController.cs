using Microsoft.AspNetCore.Mvc;
using B3cBonsai.Models;
using Microsoft.EntityFrameworkCore;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Globalization;
using System.Text;
using NuGet.ProjectModel;
using B3cBonsai.Models.ViewModels;
using B3cBonsai.Utility;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ClientProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ClientProductController> _logger;

        public ClientProductController(IUnitOfWork context, ILogger<ClientProductController> logger, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = context;
            _logger = logger;
            _userManager = userManager;
        }

        // Trang chủ: Hiển thị các sản phẩm mới nhất
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả sản phẩm, bao gồm thông tin danh mục và hình ảnh liên quan
            var products = await _unitOfWork.SanPham.GetAll(
                includeProperties: "DanhMuc,HinhAnhs",
                filter: x => x.TrangThai == true // Chỉ lấy sản phẩm có trạng thái là active
            );

            // Lấy 6 sản phẩm mới nhất
            var latestProducts = products.OrderByDescending(x => x.NgayTao).Take(6).ToList();

            // Truyền các sản phẩm mới nhất vào ViewBag để sử dụng trong view
            ViewBag.LatestProducts = latestProducts;

            return View();
        }

        // Danh sách sản phẩm có phân trang và bộ lọc
        [HttpGet]
        public async Task<IActionResult> ListPagedProduct(int? page, string? findText, int[]? selectedCategories, int? minPrice, int? maxPrice, bool? inStock, string? SortBy)
        {
            TempData["findText"] = findText ?? "";
            TempData["selectedCategories"] = selectedCategories;
            TempData["minPrice"] = minPrice ?? 0;
            TempData["maxPrice"] = maxPrice ?? 0;
            TempData["inStock"] = inStock ?? null;
            TempData["SortBy"] = SortBy;

            int pageNumber = page ?? 1;
            int pageSize = 12;

            var query = await _unitOfWork.SanPham.GetAll(
                includeProperties: "DanhMuc,HinhAnhs",
                filter: x => x.TrangThai
            );

            if (!string.IsNullOrEmpty(findText))
            {
                var normalizedFindText = RemoveDiacritics(findText.ToLower());
                query = query.Where(x => RemoveDiacritics(x.TenSanPham.ToLower()).Contains(normalizedFindText));
            }

            if (selectedCategories != null && selectedCategories.Any())
            {
                query = query.Where(x => selectedCategories.Any(select => select == x.DanhMucId));
            }

            if (minPrice.HasValue && maxPrice.HasValue && (minPrice.Value != 0 || maxPrice.Value != 0))
            {
                query = query.Where(x => x.Gia >= (minPrice ?? 0) && x.Gia <= (maxPrice ?? decimal.MaxValue));
            }

            if (inStock.HasValue)
            {
                query = inStock.Value ? query.Where(x => x.SoLuong > 0) : query.Where(x => x.SoLuong <= 0);
            }

            // Sắp xếp theo yêu cầu
            switch (SortBy)
            {
                case "best-selling":
                    query = query.OrderByDescending(x => x.SoLuong); // Giả sử có trường SoLuong bán
                    break;
                case "title-ascending":
                    query = query.OrderBy(x => x.TenSanPham);
                    break;
                case "title-descending":
                    query = query.OrderByDescending(x => x.TenSanPham);
                    break;
                case "price-ascending":
                    query = query.OrderBy(x => x.Gia);
                    break;
                case "price-descending":
                    query = query.OrderByDescending(x => x.Gia);
                    break;
                case "created-descending":
                    query = query.OrderByDescending(x => x.NgayTao);
                    break;
                case "created-ascending":
                    query = query.OrderBy(x => x.NgayTao);
                    break;
                default: // Sắp xếp theo ID (mặc định)
                    query = query.OrderBy(x => x.Id);
                    break;
            }

            var queryCombo = (await _unitOfWork.ComboSanPham.GetAll()).ToList();

            List<SanPhamOrComboVM> sanPhamOrComboVMs = new List<SanPhamOrComboVM>();
            // Gán dữ liệu hiển thị
            foreach (SanPham sanPham in query)
            {
                sanPhamOrComboVMs.Add(new SanPhamOrComboVM() { SanPham = sanPham, LoaiDoiTuong =SD.ObjectDetailOrder_SanPham });
            }
            foreach (ComboSanPham comboSanPham in queryCombo)
            {
                sanPhamOrComboVMs.Add(new SanPhamOrComboVM() { ComboSanPham = comboSanPham, LoaiDoiTuong = SD.ObjectDetailOrder_Combo });
            }
            // Phân trang kết quả
            /*var listProduct = query;*/
            var pagedListProduct = sanPhamOrComboVMs.ToPagedList(pageNumber, pageSize);
            return PartialView(pagedListProduct);
        }

        // Hàm loại bỏ dấu tiếng Việt trong chuỗi
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var character in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(character);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        // Chi tiết sản phẩm
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            // Lấy sản phẩm và các thông tin liên quan như danh mục, hình ảnh, bình luận, yêu thích
            var product = await _unitOfWork.SanPham.Get(
                includeProperties: "DanhMuc,HinhAnhs,BinhLuans,DanhSachYeuThichs",
                filter: x => x.TrangThai && x.Id == id
            );

            if (product == null)
            {
                return NotFound(); // Sản phẩm không tìm thấy
            }

            // Lấy bình luận của sản phẩm
            var comments = await _unitOfWork.BinhLuan.GetAll(
                filter: x => x.SanPhamId == id,
                includeProperties: "NguoiDungUngDung"
            );

            // Cập nhật mỗi bình luận với thông tin người dùng
            foreach (var comment in comments)
            {
                if (comment.NguoiDungUngDung?.HoTen == null)
                {
                    var user = await _userManager.FindByIdAsync(comment.NguoiDungUngDung.Id);
                    comment.NguoiDungUngDung.HoTen = user?.UserName ?? "Anonymous"; // Nếu không có tên thì mặc định là "Anonymous"
                }
            }

            ViewBag.Comments = comments;

            return View(product);
        }

        // Thêm bình luận cho sản phẩm
        [HttpPost]
        public async Task<IActionResult> AddComment(int productId, string commentContent)
        {
            try
            {
                if (string.IsNullOrEmpty(commentContent))
                {
                    return Json(new { success = false, message = "Bình luận không được để trống." });
                }

                var product = await _unitOfWork.SanPham.Get(filter: x => x.Id == productId);

                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

                var comment = new BinhLuan
                {
                    SanPhamId = productId,
                    NoiDungBinhLuan = commentContent,
                    TinhTrang = true,
                    NguoiDungId = userId
                };

                _unitOfWork.BinhLuan.Add(comment);
                _unitOfWork.Save();

                var comments = await _unitOfWork.BinhLuan.GetAll(
                    filter: x => x.SanPhamId == productId,
                    includeProperties: "NguoiDungUngDung"
                );

                return Json(new { success = true, message = "Bình luận đã được thêm thành công!", comments = comments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi xử lý bình luận.");

                return Json(new { success = false, message = "Đã xảy ra lỗi. Vui lòng thử lại." });
            }
        }

        // Xem nhanh thông tin sản phẩm
        public async Task<IActionResult> QuickView(int? id)
        {
            return PartialView(await _unitOfWork.SanPham.Get(filter: sp => sp.Id == id, includeProperties: "DanhMuc,HinhAnhs"));
        }
    }
}
