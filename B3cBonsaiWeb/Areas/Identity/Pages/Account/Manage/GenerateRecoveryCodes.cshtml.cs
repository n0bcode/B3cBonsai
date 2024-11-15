// Được cấp phép cho .NET Foundation dưới một hoặc nhiều thỏa thuận.
// .NET Foundation cấp phép cho bạn sử dụng tệp này theo giấy phép MIT.
#nullable disable

using System;
using System.Linq;
using System.Threading.Tasks;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace B3cBonsaiWeb.Areas.Identity.Pages.Account.Manage
{
    public class GenerateRecoveryCodesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<GenerateRecoveryCodesModel> _logger;

        public GenerateRecoveryCodesModel(
            UserManager<IdentityUser> userManager,
            ILogger<GenerateRecoveryCodesModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng UI mặc định của ASP.NET Core Identity và không dành cho việc sử dụng trực tiếp từ mã của bạn.
        ///     API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
        /// </summary>
        [TempData]
        public string[] RecoveryCodes { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng UI mặc định của ASP.NET Core Identity và không dành cho việc sử dụng trực tiếp từ mã của bạn.
        ///     API này có thể thay đổi hoặc bị loại bỏ trong các phiên bản tương lai.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException($"Không thể tạo mã khôi phục cho người dùng vì họ chưa bật xác thực hai yếu tố (2FA).");
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

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException($"Không thể tạo mã khôi phục cho người dùng vì họ chưa bật xác thực hai yếu tố (2FA).");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            RecoveryCodes = recoveryCodes.ToArray();

            _logger.LogInformation("Người dùng với ID '{UserId}' đã tạo mã khôi phục 2FA mới.", userId);
            StatusMessage = "Bạn đã tạo mã khôi phục mới.";
            return RedirectToPage("./ShowRecoveryCodes");
        }
    }
}
