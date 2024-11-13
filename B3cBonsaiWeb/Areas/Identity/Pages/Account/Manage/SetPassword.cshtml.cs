// Được cấp phép cho .NET Foundation theo một hoặc nhiều thỏa thuận.
// .NET Foundation cấp phép cho bạn sử dụng tệp này dưới giấy phép MIT.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace B3cBonsaiWeb.Areas.Identity.Pages.Account.Manage
{
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public SetPasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không có
        ///     mục đích được sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không có
        ///     mục đích được sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không có
        ///     mục đích được sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không có
            ///     mục đích được sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "Mật khẩu {0} phải có ít nhất {2} ký tự và tối đa {1} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không có
            ///     mục đích được sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu mới")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không khớp.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToPage("./ChangePassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Mật khẩu của bạn đã được thiết lập.";

            return RedirectToPage();
        }
    }
}
