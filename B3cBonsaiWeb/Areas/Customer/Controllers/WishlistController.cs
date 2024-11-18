using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class WishlistController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public WishlistController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var wishlistItems = (await _unitOfWork.DanhSachYeuThich.GetAll(
                filter: yt => yt.NguoiDungId == userId && yt.LoaiDoiTuong == SD.ObjectLike_SanPham,
                includeProperties: "SanPham.HinhAnhs"
            )).ToList();

            return View(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> LikeOrNot(int? objectId, string? loaiDoiTuong)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để sử dụng chức năng này!" });
            }

            if (objectId == null || string.IsNullOrEmpty(loaiDoiTuong))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            var isAlreadyLiked = await _unitOfWork.DanhSachYeuThich.Get(filter:yt =>
                yt.NguoiDungId == userId &&
                yt.LoaiDoiTuong == loaiDoiTuong &&
                ((loaiDoiTuong == SD.ObjectLike_SanPham && yt.SanPhamId == objectId) ||
                 (loaiDoiTuong == SD.ObjectLike_Comment && yt.BinhLuanId == objectId)));

            if (isAlreadyLiked != null)
            {
                _unitOfWork.DanhSachYeuThich.Remove(isAlreadyLiked);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Đã sửa trạng thái yêu thích với đối tượng." });
            }

            DanhSachYeuThich yeuThich = new DanhSachYeuThich
            {
                NguoiDungId = userId,
                LoaiDoiTuong = loaiDoiTuong
            };

            if (loaiDoiTuong == SD.ObjectLike_SanPham)
            {
                var sanPham = await _unitOfWork.SanPham.Get(filter: sp => sp.Id == objectId);
                if (sanPham == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
                }
                yeuThich.SanPhamId = objectId;
            }
            else if (loaiDoiTuong == SD.ObjectLike_Comment)
            {
                var binhLuan = await _unitOfWork.BinhLuan.Get(filter: bl => bl.Id == objectId);
                if (binhLuan == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bình luận." });
                }
                yeuThich.BinhLuanId = objectId;
            }
            else
            {
                return Json(new { success = false, message = "Loại đối tượng không hợp lệ." });
            }

            _unitOfWork.DanhSachYeuThich.Add(yeuThich);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Đã thêm đối tượng vào danh sách yêu thích." });
        }
        [HttpGet]
        public async Task<IActionResult> IsLiked(int? idObject, string? loaiDoiTuong)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra các tham số đầu vào
            if (string.IsNullOrEmpty(userId) || idObject == null || string.IsNullOrEmpty(loaiDoiTuong))
            {
                return Json(new { success = false, message = "Tham số không hợp lệ hoặc người dùng chưa đăng nhập." });
            }

            // Kiểm tra đối tượng yêu thích trong cơ sở dữ liệu
            bool isLiked = await _unitOfWork.DanhSachYeuThich.ExistsAsync(yt =>
                yt.NguoiDungId == userId &&
                yt.LoaiDoiTuong == loaiDoiTuong &&
                ((loaiDoiTuong == SD.ObjectLike_SanPham && yt.SanPhamId == idObject) ||
                 (loaiDoiTuong == SD.ObjectLike_Comment && yt.BinhLuanId == idObject)));

            return Json(new { success = true, isLiked });
        }

    }
}
