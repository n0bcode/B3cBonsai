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
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
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

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Hồ sơ của bạn đã được cập nhật.";
            return RedirectToPage();
        }
    }
}
