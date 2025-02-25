﻿// Được cấp phép cho .NET Foundation dưới một hoặc nhiều thỏa thuận.
// Quỹ .NET cấp phép cho tệp này cho bạn theo giấy phép MIT.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace B3cBonsaiWeb.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không được
        ///     thiết kế để sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành tương lai.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không được
        ///     thiết kế để sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành tương lai.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không được
        ///     thiết kế để sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành tương lai.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không được
            ///     thiết kế để sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành tương lai.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu hiện tại")]
            public string OldPassword { get; set; }

            /// <summary>
            ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không được
            ///     thiết kế để sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành tương lai.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "Trường {0} phải có độ dài tối thiểu {2} và tối đa {1} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không được
            ///     thiết kế để sử dụng trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành tương lai.
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
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
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

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("Người dùng đã thay đổi mật khẩu thành công.");
            StatusMessage = "Mật khẩu của bạn đã được thay đổi.";

            return RedirectToPage();
        }
    }
}
