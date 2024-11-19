using B3cBonsai.DataAccess.Repository.IRepository;
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
        public async Task<IActionResult> ClientOrderTable()
        {
            string? MaNguoiDung = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (MaNguoiDung == null)
            {
                return RedirectToPage("/");
            }
            return Json(new {data = (await _unitOfWork.DonHang.GetAll(filter: dh => dh.NguoiDungId == MaNguoiDung)).ToList() });
        }
        #endregion
    }
}
