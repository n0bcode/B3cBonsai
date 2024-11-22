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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet]


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
                // Loại bỏ dấu từ findText và TenSanPham trước khi tìm kiếm
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

            switch (SortBy)
            {
                case "best-selling":
                    query = query.OrderByDescending(x => x.SoLuong); // Giả sử có thuộc tính SoldCount
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
                    query = query.OrderByDescending(x => x.NgayTao); // Giả sử có thuộc tính CreatedDate
                    break;
                case "created-ascending":
                    query = query.OrderBy(x => x.NgayTao);
                    break;
                default: // "manual"
                    query = query.OrderBy(x => x.Id);
                    break;
            }



            var listProduct = query;
            var pagedListProduct = listProduct.OrderBy(x => x.Id).ToPagedList(pageNumber, pageSize);
            return PartialView(pagedListProduct);
        }


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



        // Hiển thị chi tiết sản phẩm
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _unitOfWork.SanPham.Get(
                includeProperties: "DanhMuc,HinhAnhs,BinhLuans,DanhSachYeuThichs",
                filter: x => x.TrangThai && x.Id == id
            );

            // Lấy danh sách bình luận
            var comments = await _unitOfWork.BinhLuan.GetAll(
                filter: x => x.SanPhamId == id,
                includeProperties: "NguoiDungUngDung"
            );



            foreach (var comment in comments)
            {
                // Nếu comment không có HoTen, lấy thông tin email và ảnh đại diện
                if (comment.NguoiDungUngDung?.HoTen == null)
                {
                    var user = await _userManager.FindByIdAsync(comment.NguoiDungUngDung.Id);
                    comment.NguoiDungUngDung.HoTen = user?.UserName; // Hoặc lấy email
                }
            }

            // Gửi cả thông tin sản phẩm và bình luận đến View
            ViewBag.Comments = comments;

            // Cái này để thêm vào giỏ han
            ViewBag.Product = product;
            return View(product);
        }

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

                // Cho phép NguoiDungId là null, nếu không có userId thì sẽ để giá trị null
                var comment = new BinhLuan
                {
                    SanPhamId = productId,
                    NoiDungBinhLuan = commentContent,
                    TinhTrang = true,
                    NguoiDungId = userId // Nếu không có userId, giá trị này sẽ là null
                };

                _unitOfWork.BinhLuan.Add(comment);
                _unitOfWork.Save();

                // Lấy danh sách bình luận sau khi thêm bình luận mới
                var comments = await _unitOfWork.BinhLuan.GetAll(
                    filter: x => x.SanPhamId == productId,
                    includeProperties: "NguoiDungUngDung"
                );

                // Trả về danh sách bình luận mới cùng thông báo thành công
                return Json(new { success = true, message = "Bình luận đã được thêm thành công!", comments = comments });
            }
            catch (Exception ex)
            {
                // Ghi lỗi chi tiết vào hệ thống log
                _logger.LogError(ex, "Đã xảy ra lỗi khi xử lý bình luận.");

                // Trả về thông báo lỗi cho người dùng
                return Json(new { success = false, message = "Đã xảy ra lỗi. Vui lòng thử lại." });
            }
        }



    }
}
