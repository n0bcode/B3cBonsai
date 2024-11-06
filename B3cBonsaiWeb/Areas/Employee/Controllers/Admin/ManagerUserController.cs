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

        public ManagerUserController(UserManager<IdentityUser> userManager,
                                         IUnitOfWork unitOfWork,
                                         IWebHostEnvironment webHostEnvironment,
                                         IUserStore<IdentityUser> userStore,
                                         RoleManager<IdentityRole> roleManager,
                                         ILogger<ManagerUserController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _userStore = userStore;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(string? id)
        {
            NguoiDungUngDungVM nguoiDungUngDungVM = new NguoiDungUngDungVM();
            nguoiDungUngDungVM.DanhSachVaiTro = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            });
            if (id == null)
            {
                nguoiDungUngDungVM.NguoiDungUngDung = new NguoiDungUngDung();
                return PartialView(nguoiDungUngDungVM);
            }


            var nguoiDungUngDung = _unitOfWork.NguoiDungUngDung.Get(x => x.Id == id);
            if (nguoiDungUngDung == null)
            {
                return NotFound();
            }
            nguoiDungUngDungVM.NguoiDungUngDung = nguoiDungUngDung;
            return PartialView(nguoiDungUngDungVM);
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(NguoiDungUngDungVM nguoi, IFormFile? file, string? VaiTro)
        {
            if (ModelState.IsValid)
            {
                NguoiDungUngDung user = (NguoiDungUngDung)await _userManager.FindByIdAsync(nguoi.NguoiDungUngDung.Id) ?? CreateUser();

                if (user.Id == null) // User mới
                {
                    await _userStore.SetUserNameAsync(user, nguoi.NguoiDungUngDung.UserName, CancellationToken.None);
                    await _userManager.SetEmailAsync(user, nguoi.NguoiDungUngDung.Email);

                    var createUserResult = await _userManager.CreateAsync(user, nguoi.NguoiDungUngDung.PasswordHash);
                    if (!createUserResult.Succeeded)
                    {
                        foreach (var error in createUserResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        return View(nguoi);
                    }

                    if (!string.IsNullOrEmpty(VaiTro))
                        await _userManager.AddToRoleAsync(user, VaiTro);
                    else
                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);

                    nguoi.NguoiDungUngDung.Id = user.Id;
                }
                else
                {
                    try
                    {
                        var existingUser = _unitOfWork.NguoiDungUngDung.Get(x => x.Id == nguoi.NguoiDungUngDung.Id);
                        if (existingUser != null && existingUser.ConcurrencyStamp != nguoi.NguoiDungUngDung.ConcurrencyStamp)
                        {
                            ModelState.AddModelError(string.Empty, "Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang.");
                            return Json(new { success = false, title = "Lỗi xác thực", content = "Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang." });
                        }

                        _unitOfWork.NguoiDungUngDung.Update(nguoi.NguoiDungUngDung);
                        _unitOfWork.Save();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        _logger.LogError(ex, "Lỗi đồng thời trong khi cập nhật dữ liệu.");
                        ModelState.AddModelError(string.Empty, "Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng thử lại.");
                        return Json(new { success = false, title = "Lỗi đồng thời trong khi cập nhật dữ liệu.", content = "Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng thử lại." });
                    }
                }

                // Xử lý ảnh
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string nguoiDungUngDungPath = Path.Combine("images", "user", "user-" + nguoi.NguoiDungUngDung.Id);
                    string finalPath = Path.Combine(wwwRootPath, nguoiDungUngDungPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    if (!string.IsNullOrEmpty(nguoi.NguoiDungUngDung.LinkAnh))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, nguoi.NguoiDungUngDung.LinkAnh);
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    nguoi.NguoiDungUngDung.LinkAnh = Path.Combine(nguoiDungUngDungPath, fileName).Replace("\\", "/");
                    _unitOfWork.NguoiDungUngDung.Update(nguoi.NguoiDungUngDung);
                    _unitOfWork.Save();
                }

                TempData["successToastr"] = "NguoiDungUngDung created/updated successfully";
                return Json(new { success = true, title = "Thông báo", content = "Tạo thông tin thành công." });
            }

            nguoi.DanhSachVaiTro = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            });
            var viewHtml = await this.RenderViewAsync("Upsert", nguoi, true);
            return Json(new { success = false, title = "Lỗi xác thực", content = "Dữ liệu không hợp lệ.", data = viewHtml });
        }

        public IActionResult DetailWithDelete(string? id)
        {
            NguoiDungUngDung nguoiDungUngDung = _unitOfWork.NguoiDungUngDung.Get(x => x.Id == id);
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
                nguoiDungUngDungs.Where(nd => nd.VaiTro == SD.Role_Customer).ToList();
            }
            return Json(new { data = nguoiDungUngDungs });
        }
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "ID không hợp lệ." });
            }

            NguoiDungUngDung user = _unitOfWork.NguoiDungUngDung.Get(x => x.Id == id);
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

            if (_userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault() == SD.Role_Admin)
            {
                return Json(new { success = false, message = "Bạn không thể xóa thông tin người dùng này." });
            }

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
                return Json(new { success = false, message = "Lỗi xảy ra khi xóa người dùng." });
            }

            _logger.LogInformation("Người dùng với ID '{UserId}' đã bị xóa.", id);
            return Json(new { success = true, message = "Xóa thành công." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, string newRole)
        {
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
    }
}
