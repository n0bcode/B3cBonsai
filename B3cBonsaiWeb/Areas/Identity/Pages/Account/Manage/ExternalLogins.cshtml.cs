// Được cấp phép cho .NET Foundation theo một hoặc nhiều thỏa thuận.
// .NET Foundation cấp phép tệp này cho bạn theo giấy phép MIT.
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace B3cBonsaiWeb.Areas.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserStore<IdentityUser> _userStore;

        public ExternalLoginsModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUserStore<IdentityUser> userStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
        }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không có mục đích sử dụng trực tiếp
        ///     từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành sau.
        /// </summary>
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không có mục đích sử dụng trực tiếp
        ///     từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành sau.
        /// </summary>
        public IList<AuthenticationScheme> OtherLogins { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không có mục đích sử dụng trực tiếp
        ///     từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành sau.
        /// </summary>
        public bool ShowRemoveButton { get; set; }

        /// <summary>
        ///     API này hỗ trợ cơ sở hạ tầng giao diện người dùng mặc định của ASP.NET Core Identity và không có mục đích sử dụng trực tiếp
        ///     từ mã của bạn. API này có thể thay đổi hoặc bị loại bỏ trong các bản phát hành sau.
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

            CurrentLogins = await _userManager.GetLoginsAsync(user);
            OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();

            string passwordHash = null;
            if (_userStore is IUserPasswordStore<IdentityUser> userPasswordStore)
            {
                passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
            }

            ShowRemoveButton = passwordHash != null || CurrentLogins.Count > 1;
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                StatusMessage = "Đăng nhập ngoài đã không được xóa.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Đăng nhập ngoài đã được xóa.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Xóa cookie ngoài hiện tại để đảm bảo quy trình đăng nhập sạch sẽ
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Yêu cầu chuyển hướng tới nhà cung cấp đăng nhập ngoài để liên kết một đăng nhập cho người dùng hiện tại
            var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var info = await _signInManager.GetExternalLoginInfoAsync(userId);
            if (info == null)
            {
                throw new InvalidOperationException($"Đã xảy ra lỗi không mong muốn khi tải thông tin đăng nhập ngoài.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                StatusMessage = "Đăng nhập ngoài không thể được thêm. Các đăng nhập ngoài chỉ có thể được liên kết với một tài khoản.";
                return RedirectToPage();
            }

            // Xóa cookie ngoài hiện tại để đảm bảo quy trình đăng nhập sạch sẽ
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "Đăng nhập ngoài đã được thêm.";
            return RedirectToPage();
        }
    }
}
