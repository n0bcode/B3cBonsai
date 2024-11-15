// Cấp phép cho .NET Foundation theo một hoặc nhiều thỏa thuận.
// .NET Foundation cấp phép cho tệp này cho bạn theo giấy phép MIT.
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
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<Disable2faModel> _logger;

        public Disable2faModel(
            UserManager<IdentityUser> userManager,
            ILogger<Disable2faModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException($"Không thể vô hiệu hóa 2FA cho người dùng vì tính năng này chưa được bật.");
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

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException($"Đã xảy ra lỗi không mong muốn khi vô hiệu hóa 2FA.");
            }

            _logger.LogInformation("Người dùng với ID '{UserId}' đã vô hiệu hóa 2FA.", _userManager.GetUserId(User));
            StatusMessage = "2FA đã được vô hiệu hóa. Bạn có thể bật lại 2FA khi thiết lập ứng dụng xác thực.";
            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}
