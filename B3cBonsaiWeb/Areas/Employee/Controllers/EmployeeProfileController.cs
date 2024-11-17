using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        public EmployeeProfileController(UserManager<IdentityUser> userManager,
                                         IUnitOfWork unitOfWork,
                                         IWebHostEnvironment webHostEnvironment,
                                         ApplicationDbContext db,
                                         IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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
                _unitOfWork.NguoiDungUngDung.Update(nguoi);
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

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                var nguoi = await _unitOfWork.NguoiDungUngDung.Get(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string nguoiDungUngDungPath = Path.Combine("images", "user", "user-" + nguoi.Id);
                string finalPath = Path.Combine(wwwRootPath, nguoiDungUngDungPath);

                if (!Directory.Exists(finalPath))
                    Directory.CreateDirectory(finalPath);

                if (!string.IsNullOrEmpty(nguoi.LinkAnh))
                {
                    string oldImagePath = Path.Combine(wwwRootPath, nguoi.LinkAnh);
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                nguoi.LinkAnh = '/' + Path.Combine(nguoiDungUngDungPath, fileName).Replace("\\", "/");
                _unitOfWork.NguoiDungUngDung.Update(nguoi);
                _unitOfWork.Save();
                return Json(new { success = true });
            }
            catch (Exception ex) {
                return Json(new { success = false, content = ex.Message }); 
            }
        }
    }
}
