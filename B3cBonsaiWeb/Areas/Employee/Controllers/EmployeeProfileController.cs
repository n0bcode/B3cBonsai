using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using B3cBonsai.Utility.Extentions;
using B3cBonsai.Utility;

namespace B3cBonsaiWeb.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class EmployeeProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        public EmployeeProfileController(UserManager<IdentityUser> userManager,
                                         IUnitOfWork unitOfWork,
                                         ApplicationDbContext db,
                                         IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _db = db;
            _contextAccessor = httpContextAccessor;
        }
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Staff}")]
        public async Task<IActionResult> Index()
        {
            var nguoi = await _unitOfWork.NguoiDungUngDung.Get(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            nguoi.VaiTro = User.FindFirstValue(ClaimTypes.Role);
            return View(nguoi);
        }
        [Authorize]
        public async Task<IActionResult> ProfileForm(NguoiDungUngDung nguoi)
        {
            return PartialView(nguoi);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProfileFormUpdate(NguoiDungUngDung nguoi)
        {
            var currentUser = await _unitOfWork.NguoiDungUngDung.Get(nd => nd.Id == nguoi.Id);

            if (!ModelState.IsValid)
            {
                var viewHtml = await this.RenderViewAsync("ProfileForm", nguoi, true);
                return Json(new { success = false, data = viewHtml });
            }

            if (currentUser?.CCCD != nguoi.CCCD &&
                await _unitOfWork.NguoiDungUngDung.Get(nd => nd.CCCD == nguoi.CCCD) != null)
            {
                ModelState.AddModelError("CCCD", "Số mã CCCD đã có trong hệ thống!");
            }

            if (currentUser?.Email != nguoi.Email &&
                await _unitOfWork.NguoiDungUngDung.Get(nd => nd.Email == nguoi.Email) != null)
            {
                ModelState.AddModelError("Email", "Email đã có trong hệ thống!");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.NguoiDungUngDung.UpdateUserInfoAndImage(nguoi, null);
                await _db.SaveChangesAsync();

                var viewHtml = await this.RenderViewAsync("ProfileForm", nguoi, true);
                return Json(new { success = true, data = viewHtml });
            }

            var errorViewHtml = await this.RenderViewAsync("ProfileForm", nguoi, true);
            return Json(new { success = false, data = errorViewHtml });
        }

        public async Task<IActionResult> ChangeAvatar(IFormFile? file)
        {
            try
            {
                if (file == null)
                {
                    return Json(new { success = false, content = "Lỗi nhận nhận dữ liệu hình ảnh!" });
                }

                var nguoi = await _unitOfWork.NguoiDungUngDung.Get(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _unitOfWork.NguoiDungUngDung.UpdateUserInfoAndImage(nguoi, file);
                _unitOfWork.Save();
                return Json(new { success = true });
            }
            catch (Exception ex) {
                return Json(new { success = false, content = ex.Message }); 
            }
        }
    }
}
