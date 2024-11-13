// Được cấp phép cho .NET Foundation theo một hoặc nhiều thỏa thuận.
// .NET Foundation cấp phép tệp này cho bạn theo giấy phép MIT.
#nullable disable

using System;
using System.Threading.Tasks;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace B3cBonsaiWeb.Areas.Identity.Pages.Account.Manage
{
    public class ResetAuthenticatorModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ResetAuthenticatorModel> _logger;

        public ResetAuthenticatorModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ResetAuthenticatorModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng UI mặc định của ASP.NET Core Identity và không được thiết kế để sử dụng
        ///     trực tiếp từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành sau.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            _logger.LogInformation("Người dùng với ID '{UserId}' đã đặt lại khóa ứng dụng xác thực của họ.", user.Id);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Khóa ứng dụng xác thực của bạn đã được đặt lại, bạn sẽ cần cấu hình lại ứng dụng xác thực của mình với khóa mới.";

            return RedirectToPage("./EnableAuthenticator");
        }
    }
}
