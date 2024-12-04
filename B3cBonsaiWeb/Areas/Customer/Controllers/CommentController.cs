using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            // Fetch comments for the specified product, including user properties in a single query
            var comments = (await _unitOfWork.BinhLuan.GetAll(
                filter: x => x.SanPhamId == productId,
                includeProperties: "NguoiDungUngDung"
            )).OrderBy(x => x.NgayBinhLuan).ToList();

            // Get all user IDs from the comments
            var userIds = comments
                .Select(comment => comment.NguoiDungUngDung.Id)
                .Distinct() // Ensure unique user IDs to minimize calls
                .ToList();

            // Load users in a single call
            var users = await _userManager.Users
                .Where(user => userIds.Contains(user.Id))
                .ToListAsync();

            // Create a dictionary for quick lookup of users by their ID
            var userDictionary = users.ToDictionary(user => user.Id, user => user);

            // Update comments with user information
            foreach (var comment in comments)
            {
                if (comment.NguoiDungUngDung != null) // Ensure that the associated user is not null
                {
                    var userId = comment.NguoiDungUngDung.Id;
                    if (userDictionary.ContainsKey(userId))
                    {
                        var user = userDictionary[userId];
                        comment.NguoiDungUngDung.HoTen = user?.UserName ?? "Anonymous";
                        comment.NguoiDungUngDung.VaiTro = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "No Role"; // If not available, set to a default value
                    }
                    else
                    {
                        comment.NguoiDungUngDung.HoTen = "Anonymous"; // Fallback if user not found
                        comment.NguoiDungUngDung.VaiTro = "No Role";
                    }
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

        // Xóa bình luận
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

                var comment = await _unitOfWork.BinhLuan.Get(filter: x => x.Id == commentId);
                if (comment == null)
                {
                    return Json(new { success = false, message = "Bình luận không tồn tại." });
                }

                // Kiểm tra quyền xóa
                if (comment.NguoiDungId != userId && !User.IsInRole("Admin"))
                {
                    return Json(new { success = false, message = "Bạn không có quyền xóa bình luận này." });
                }

                // Thực hiện xóa
                _unitOfWork.BinhLuan.Remove(comment);
                _unitOfWork.Save();

                return Json(new { success = true, message = "Bình luận đã được xóa thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa bình luận ID: {commentId}");
                return Json(new { success = false, message = "Đã xảy ra lỗi. Vui lòng thử lại." });
            }
        }


    }
}
