using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsaiWeb.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    //Quản lý các chức năng liên quan đến thông tin người dùng
    public class ClientProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ClientProfileController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult ProAddress()
        {
            return View();
        }
        public IActionResult ProWishlist()
        {
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        public IActionResult ProTickets()
        {
            return View();
        }

        #region//GET APIS
        [Authorize]
        [CheckUserStatus]
        public async Task<IActionResult> ClientOrderData()
        {
            string? MaNguoiDung = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (MaNguoiDung == null)
            {
                return RedirectToPage("/");
            }
            return Json(new { data = (await _unitOfWork.DonHang.GetAll(filter: dh => dh.NguoiDungId == MaNguoiDung)).ToList() });
        }
        [Authorize]
        public async Task<IActionResult> ClientCommentData()
        {
            string? MaNguoiDung = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (MaNguoiDung == null)
            {
                return RedirectToPage("/");
            }

            var listComments = await _unitOfWork.BinhLuan.GetAll(
                filter: dh => dh.NguoiDungId == MaNguoiDung,
                includeProperties: "SanPham.HinhAnhs"
            );

            if (listComments == null || !listComments.Any())
            {
                return Json(new { data = new List<object>() }); // Return an empty data list
            }

            foreach (var comment in listComments)
            {
                comment.SanPham = new SanPham
                {
                    Id = comment.SanPham.Id,
                    TenSanPham = comment.SanPham.TenSanPham,
                    HinhAnhs = comment.SanPham.HinhAnhs.Select(ha => new HinhAnhSanPham { LinkAnh = ha.LinkAnh }).ToList()
                };
            }

            return Json(new { data = listComments });
        }

        #endregion
    }
}
