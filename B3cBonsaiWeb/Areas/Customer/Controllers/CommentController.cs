using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ClientProductController> _logger;

        public CommentController(IUnitOfWork context, ILogger<ClientProductController> logger, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = context;
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int productId)
        {
            var comments = (await _unitOfWork.BinhLuan.GetAll(
                filter: x => x.SanPhamId == productId,
                includeProperties: "NguoiDungUngDung"
            )).OrderBy(x => x.NgayBinhLuan);

            // Cập nhật mỗi bình luận với thông tin người dùng
            foreach (var comment in comments)
            {
                if (comment.NguoiDungUngDung?.HoTen == null)
                {
                    var user = await _userManager.FindByIdAsync(comment.NguoiDungUngDung.Id);
                    comment.NguoiDungUngDung.HoTen = user?.UserName ?? "Anonymous"; // Nếu không có tên thì mặc định là "Anonymous"
                }
            }
            return PartialView(comments);
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

                return Json(new { success = true, message = "Bình luận đã được thêm thành công!"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi xử lý bình luận.");

                return Json(new { success = false, message = "Đã xảy ra lỗi. Vui lòng thử lại." });
            }
        }

    }
}
