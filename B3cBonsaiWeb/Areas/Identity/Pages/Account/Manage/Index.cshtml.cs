// Được cấp phép cho .NET Foundation dưới một hoặc nhiều thỏa thuận.
// .NET Foundation cấp phép cho bạn sử dụng tệp này theo giấy phép MIT.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace B3cBonsaiWeb.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng UI mặc định của ASP.NET Core Identity và không dành cho việc sử dụng trực tiếp từ mã của bạn.
        ///     API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng UI mặc định của ASP.NET Core Identity và không dành cho việc sử dụng trực tiếp từ mã của bạn.
        ///     API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng UI mặc định của ASP.NET Core Identity và không dành cho việc sử dụng trực tiếp từ mã của bạn.
        ///     API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng UI mặc định của ASP.NET Core Identity và không dành cho việc sử dụng trực tiếp từ mã của bạn.
        ///     API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     API này hỗ trợ cơ sở hạ tầng UI mặc định của ASP.NET Core Identity và không dành cho việc sử dụng trực tiếp từ mã của bạn.
            ///     API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
            /// </summary>
            [Phone]
            [StringLength(18, ErrorMessage = "Số điện thoại không được vượt quá 18 ký tự.")]
            [Display(Name = "Số Điện Thoại")]
            [RegularExpression(@"^\+?\d{1,3}?[-.\s]?\(?\d{1,3}\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
            public string PhoneNumber { get; set; }


            [Required(ErrorMessage = "Họ tên không được để trống.")]
            [StringLength(54, ErrorMessage = "Họ tên không được vượt quá 54 ký tự.")]
            [Display(Name = "Họ Tên")]
            [RegularExpression(SD.ValidateStringName, ErrorMessage = "Họ tên chỉ được chứa chữ cái và khoảng trắng.")]
            public string HoTen { get; set; }

            [Display(Name = "Ngày Sinh")]
            public DateTime? NgaySinh { get; set; }

            [Display(Name = "Giới Tính")]
            public bool? GioiTinh { get; set; }

            [StringLength(18, ErrorMessage = "CCCD không được vượt quá 18 ký tự.", MinimumLength = 12)]
            [Display(Name = "Số CCCD")]
            [RegularExpression(@"^\d+$", ErrorMessage = "Số CCCD chỉ được chứa số.")]
            public string? CCCD { get; set; } // Số CCCD cần unique

            [StringLength(1024, ErrorMessage = "Địa chỉ không được vượt quá 1024 ký tự.")]
            [Display(Name = "Địa Chỉ")]
            [RegularExpression(SD.ValidateString, ErrorMessage = "Địa chỉ chỉ được chứa chữ cái, số và khoảng trắng.")]
            [Required(ErrorMessage = "Vui lòng nhập thông tin địa chỉ người dùng.")]
            public string DiaChi { get; set; }
        }
        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            // Giả sử các thông tin bổ sung được lưu trong lớp tùy chỉnh ApplicationUser
            var customUser = user as NguoiDungUngDung; // Thay ApplicationUser bằng lớp người dùng tùy chỉnh của bạn nếu có

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                HoTen = customUser?.HoTen,
                NgaySinh = customUser?.NgaySinh,
                GioiTinh = customUser?.GioiTinh,
                CCCD = customUser?.CCCD,
                DiaChi = customUser?.DiaChi
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Cập nhật số điện thoại
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Lỗi không mong đợi khi cố gắng thay đổi số điện thoại.";
                    return RedirectToPage();
                }
            }

            // Cập nhật thông tin bổ sung (giả sử các thông tin này được lưu trong `IdentityUser` hoặc bảng liên kết)
            var customUser = user as NguoiDungUngDung; // Ép kiểu nếu bạn có lớp người dùng tùy chỉnh
            if (customUser != null)
            {
                customUser.HoTen = Input.HoTen;
                customUser.NgaySinh = Input.NgaySinh;
                customUser.GioiTinh = Input.GioiTinh;
                customUser.CCCD = Input.CCCD;
                customUser.DiaChi = Input.DiaChi;

                var updateResult = await _userManager.UpdateAsync(customUser);
                if (!updateResult.Succeeded)
                {
                    StatusMessage = "Lỗi khi cập nhật thông tin người dùng.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Hồ sơ của bạn đã được cập nhật.";
            return RedirectToPage();
        }

    }
}
