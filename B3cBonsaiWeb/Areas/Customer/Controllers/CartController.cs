using B3cBonsai.DataAccess.Repository;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using B3cBonsai.Utility.Extentions;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    // Quản lý tính năng liên quan đến giỏ hàng
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            string? maKhachHang = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = GetCartItems(maKhachHang);
            foreach (var item in cartItems)
            {
                if (item.LoaiDoiTuong == SD.ObjectDetailOrder_Combo)
                {
                    item.ComboSanPham = await _unitOfWork.ComboSanPham.Get(filter: cbo => cbo.Id == item.MaCombo);
                } else
                {
                    item.SanPham = await _unitOfWork.SanPham.Get(filter: cbo => cbo.Id == item.MaSanPham);
                }
            }
            return View(cartItems);
        }

        #region // Tính năng chủ yếu của giỏ hàng
        // Helper method for retrieving cart items
        private IList<GioHang> GetCartItems(string maKhachHang)
        {
            return maKhachHang == null
                ? new List<GioHang>()
                : HttpContext.Session.GetComplexData<IEnumerable<GioHang>>(SD.SessionCart)
                    ?.Where(x => x.MaKhachHang == maKhachHang).ToList() ?? new List<GioHang>();
        }
        [HttpPost]
        public async Task<IActionResult> Plus(int cartId)
        {
            string? maKhachHang = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = GetCartItems(maKhachHang);
            var item = cartItems.FirstOrDefault(x => x.Id == cartId);
            if (item != null)
            {
                item.SoLuong++;
                HttpContext.Session.SetComplexData(SD.SessionCart, cartItems);
                return Json(new { success = true, total = item.SoLuong * item.Gia });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
        }

        [HttpPost]
        public async Task<IActionResult> Minus(int cartId)
        {
            string? maKhachHang = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = GetCartItems(maKhachHang);
            var item = cartItems.FirstOrDefault(x => x.Id == cartId);
            if (item != null)
            {
                if (item.SoLuong > 1)
                {
                    item.SoLuong--;
                    HttpContext.Session.SetComplexData(SD.SessionCart, cartItems);
                    return Json(new { success = true, total = item.SoLuong * item.Gia });
                }
                return Json(new { success = false, message = "Số lượng không thể giảm dưới 1." });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
        }
        [HttpPost]
        public async Task<IActionResult> Add(int? sanPhamId, int? comboId, int? soLuong, string? loaiDoiTuong)
        {
            string? maKhachHang = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = GetCartItems(maKhachHang);
            if (maKhachHang == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng." });
            }
            if (User.FindFirstValue(ClaimTypes.Role) != SD.Role_Customer)
            {
                return Json(new { success = false, message = "Bạn không phải là khách hàng để thêm sản phẩm vào giỏ hàng." });
            }
            // Khởi tạo số lượng mặc định là 0 nếu không có giá trị
            soLuong = soLuong ?? 0;

            // Nếu loại đối tượng không tồn tại, trả về kết quả trống
            if (string.IsNullOrEmpty(loaiDoiTuong))
            {
                return Json(new { success = false, message = "Không xác nhận được đối tượng bạn vừa thêm." });
            }

            // Xử lý sản phẩm
            if (loaiDoiTuong == SD.ObjectDetailOrder_SanPham)
            {
                if (sanPhamId == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong hệ thống." });
                }

                SanPham? sanPham = await _unitOfWork.SanPham.Get(filter: sp => sp.Id == sanPhamId);
                if (sanPham == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong hệ thống." });
                }

                if (!sanPham.TrangThai)
                {
                    return Json(new { success = false, message = "Sản phẩm không còn khả dụng." });
                }
                cartItems.Add(new GioHang() {Id = new Random().Next(0,int.MaxValue), MaSanPham = comboId, SoLuong = soLuong.Value, MaKhachHang = maKhachHang, SanPham = sanPham, Gia = sanPham.Gia, LoaiDoiTuong = loaiDoiTuong });
            }
            // Xử lý combo sản phẩm
            else if (loaiDoiTuong == SD.ObjectDetailOrder_Combo)
            {
                if (comboId == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy combo sản phẩm trong hệ thống." });
                }

                ComboSanPham? comboSanPham = await _unitOfWork.ComboSanPham.Get(filter: sp => sp.Id == comboId);
                if (comboSanPham == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy combo sản phẩm trong hệ thống." });
                }

                if (!comboSanPham.TrangThai)
                {
                    return Json(new { success = false, message = "Combo sản phẩm không còn khả dụng." });
                }
                cartItems.Add(new GioHang() { Id = new Random().Next(0, int.MaxValue), MaCombo = comboId, SoLuong = soLuong.Value, MaKhachHang = maKhachHang, ComboSanPham = comboSanPham, Gia = comboSanPham.TongGia, LoaiDoiTuong = loaiDoiTuong });
            }
            else
            {
                return Json(new { success = false, message = "Loại đối tượng không hợp lệ." });
            }

            // Kiểm tra số lượng hợp lệ
            if (soLuong <= 0)
            {
                return Json(new { success = false, message = "Số lượng phải lớn hơn 0." });
            }
            HttpContext.Session.SetComplexData(SD.SessionCart, cartItems);
            // Trả về thành công nếu không có lỗi
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> Remove(int cartId)
        {
            string? maKhachHang = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = GetCartItems(maKhachHang);
            var item = cartItems.FirstOrDefault(x => x.Id == cartId);
            if (item != null)
            {
                cartItems.Remove(item);
                HttpContext.Session.SetComplexData(SD.SessionCart, cartItems);
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm để xóa." });
        }
        #endregion
    }
}
