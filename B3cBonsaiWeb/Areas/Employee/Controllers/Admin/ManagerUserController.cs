using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using B3cBonsai.DataAccess.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;
using B3cBonsai.Utility.Extentions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using B3cBonsai.Utility;
using B3cBonsai.Models.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Admin
{
    [Area("Employee")]
    public class ManagerUserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ManagerUserController> _logger;
        private readonly IEmailSender _emailSender;
        public ManagerUserController(UserManager<IdentityUser> userManager,
                                         IUnitOfWork unitOfWork,
                                         IWebHostEnvironment webHostEnvironment,
                                         IUserStore<IdentityUser> userStore,
                                         RoleManager<IdentityRole> roleManager,
                                         ILogger<ManagerUserController> logger,
                                         IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _userStore = userStore;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _emailSender = emailSender;
        }

        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Staff}")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(string? id)
        {
            var nguoiDungUngDungVM = new NguoiDungUngDungVM
            {
                DanhSachVaiTro = GetRoleList()
            };

            if (string.IsNullOrEmpty(id))
            {
                nguoiDungUngDungVM.NguoiDungUngDung = new NguoiDungUngDung { ThaoTac = true };
                return PartialView(nguoiDungUngDungVM);
            }

            var nguoiDungUngDung = await _unitOfWork.NguoiDungUngDung.Get(x => x.Id == id);
            if (nguoiDungUngDung == null)
                return NotFound();

            nguoiDungUngDungVM.NguoiDungUngDung = nguoiDungUngDung;
            nguoiDungUngDungVM.NguoiDungUngDung.ThaoTac = false;
            return PartialView(nguoiDungUngDungVM);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(NguoiDungUngDungVM nguoi, IFormFile? file, string? VaiTro)
        {
            var existingUser = await _unitOfWork.NguoiDungUngDung.Get(x => x.Id == nguoi.NguoiDungUngDung.Id);
            var isCreatingNewUser = nguoi.NguoiDungUngDung.ThaoTac;

            // Check if the user already exists based on CCCD and Email
            if (isCreatingNewUser ||
               (existingUser?.CCCD != nguoi.NguoiDungUngDung.CCCD &&
                await _unitOfWork.NguoiDungUngDung.Get(x => x.CCCD == nguoi.NguoiDungUngDung.CCCD && x.Id != nguoi.NguoiDungUngDung.Id) != null))
            {
                ModelState.AddModelError("CCCD", "Số mã CCCD đã có trong hệ thống!");
            }

            if (isCreatingNewUser ||
               (existingUser?.Email != nguoi.NguoiDungUngDung.Email &&
                await _unitOfWork.NguoiDungUngDung.Get(x => x.Email == nguoi.NguoiDungUngDung.Email && x.Id != nguoi.NguoiDungUngDung.Id) != null))
            {
                ModelState.AddModelError("Email", "Email đã có trong hệ thống!");
            }

            if (!ModelState.IsValid)
            {
                nguoi.DanhSachVaiTro = GetRoleList();
                var viewHtml = await this.RenderViewAsync("Upsert", nguoi, true);
                return Json(new { success = false, title = "Lỗi xác thực", content = "Dữ liệu không hợp lệ.", data = viewHtml });
            }

            if (isCreatingNewUser)
            {
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, nguoi.NguoiDungUngDung.UserName, CancellationToken.None);
                await _userManager.SetEmailAsync(user, nguoi.NguoiDungUngDung.Email);

                user.HoTen = nguoi.NguoiDungUngDung.HoTen;
                user.SoDienThoai = nguoi.NguoiDungUngDung.SoDienThoai;
                user.NgaySinh = nguoi.NguoiDungUngDung.NgaySinh;
                user.DiaChi = nguoi.NguoiDungUngDung.DiaChi;
                user.GioiTinh = nguoi.NguoiDungUngDung.GioiTinh;
                user.NgayTao = nguoi.NguoiDungUngDung.NgayTao;
                user.MoTa = nguoi.NguoiDungUngDung.MoTa;

                var createUserResult = await _userManager.CreateAsync(user, nguoi.NguoiDungUngDung.DatMatKhau);
                if (!createUserResult.Succeeded)
                {
                    foreach (var error in createUserResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    nguoi.DanhSachVaiTro = GetRoleList();
                    var viewHtml = await this.RenderViewAsync("Upsert", nguoi, true);
                    return Json(new { success = false, title = "Lỗi xác thực", content = "Dữ liệu không hợp lệ.", data = viewHtml });
                }

                var roleToAssign = string.IsNullOrEmpty(VaiTro) ? SD.Role_Customer : VaiTro;
                await _userManager.AddToRoleAsync(user, roleToAssign);
            }
            else
            {
                _unitOfWork.NguoiDungUngDung.UpdateUserInfoAndImage(nguoi.NguoiDungUngDung, file);
            }

            _unitOfWork.Save();

            return Json(new { success = true, title = "Thông báo", content = "Cập nhật thông tin thành công." });
        }

        private async Task<string> SaveUserImage(IFormFile file, string userId)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string nguoiDungUngDungPath = Path.Combine("images", "user", "user-" + userId);
            string finalPath = Path.Combine(wwwRootPath, nguoiDungUngDungPath);

            if (!Directory.Exists(finalPath))
                Directory.CreateDirectory(finalPath);

            string newImagePath = Path.Combine(finalPath, fileName);

            if (!string.IsNullOrEmpty(finalPath))
            {
                string oldImagePath = Path.Combine(wwwRootPath, finalPath);
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }

            using (var fileStream = new FileStream(newImagePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/" + Path.Combine(nguoiDungUngDungPath, fileName).Replace("\\", "/");
        }

        public async Task<IActionResult> DetailWithDelete(string? id)
        {
            NguoiDungUngDung nguoiDungUngDung = await _unitOfWork.NguoiDungUngDung.Get(x => x.Id == id);
            if (nguoiDungUngDung == null)
            {
                return NotFound();
            }
            return PartialView(nguoiDungUngDung);
        }

        #region GET API
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<NguoiDungUngDung> nguoiDungUngDungs = await _unitOfWork.NguoiDungUngDung.GetAll();


            foreach (var user in nguoiDungUngDungs)
            {
                user.VaiTro = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
            }
            if (!User.IsInRole(SD.Role_Admin))
            {
                nguoiDungUngDungs = nguoiDungUngDungs.Where(nd => nd.VaiTro == SD.Role_Customer).ToList();
            }
            return Json(new { data = nguoiDungUngDungs });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { success = false, message = "ID không hợp lệ." });
            }

            NguoiDungUngDung user = await _unitOfWork.NguoiDungUngDung.Get(x => x.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tìm thấy." });
            }

            // Check if the user is trying to delete their own account
            string currentUserId = _userManager.GetUserId(User);
            if (currentUserId == id)
            {
                return Json(new { success = false, message = "Bạn không thể xóa tài khoản của chính mình." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(SD.Role_Admin))
            {
                return Json(new { success = false, message = "Bạn không thể xóa thông tin người dùng này." });
            }

            // Xóa thư mục chứa hình ảnh người dùng
            string userPath = Path.Combine("images", "user", $"user-{id}");
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, userPath);

            if (Directory.Exists(finalPath))
            {
                var filePaths = Directory.GetFiles(finalPath);
                foreach (var filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Lỗi xảy ra khi xóa người dùng. Vui lòng kiểm tra lại." });
            }

            _logger.LogInformation("Người dùng với ID '{UserId}' đã bị xóa.", id);
            return Json(new { success = true, message = "Xóa người dùng thành công." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, string newRole)
        {
            if (!User.IsInRole(SD.Role_Admin))
            {
                return Json(new { success = false, message = "Bạn không có quyền này." });
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng." });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (!removeRolesResult.Succeeded)
            {
                return Json(new { success = false, message = "Lỗi khi xóa vai trò cũ của người dùng." });
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addRoleResult.Succeeded)
            {
                return Json(new { success = false, message = "Lỗi khi thêm vai trò mới cho người dùng." });
            }

            return Json(new { success = true, message = "Cập nhật vai trò thành công." });
        }

        // Thêm hàm khóa hoặc mở khóa tài khoản
        [HttpPost]
        public async Task<IActionResult> LockOrUnlockUser(string userId, bool isLock)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng." });
            }

            if (isLock)
            {
                // Khóa tài khoản trong 1 năm
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(1);
            }
            else
            {
                // Mở khóa tài khoản
                user.LockoutEnd = null;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Json(new { success = false, message = "Lỗi khi cập nhật trạng thái tài khoản." });
            }

            return Json(new { success = true, message = isLock ? "Khóa tài khoản thành công." : "Mở khóa tài khoản thành công." });
        }
        #endregion

        private NguoiDungUngDung CreateUser()
        {
            try
            {
                return Activator.CreateInstance<NguoiDungUngDung>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        private List<SelectListItem> GetRoleList() =>
            _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            }).ToList();
    }
}
