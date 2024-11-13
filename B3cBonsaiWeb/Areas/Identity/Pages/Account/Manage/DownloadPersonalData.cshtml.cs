// Được cấp phép cho .NET Foundation dưới một hoặc nhiều thỏa thuận.
// .NET Foundation cấp phép cho bạn sử dụng tệp này dưới giấy phép MIT.
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace B3cBonsaiWeb.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;

        public DownloadPersonalDataModel(
            UserManager<IdentityUser> userManager,
            ILogger<DownloadPersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("Người dùng với ID '{UserId}' yêu cầu dữ liệu cá nhân của họ.", _userManager.GetUserId(User));

            // Chỉ bao gồm dữ liệu cá nhân để tải xuống
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(IdentityUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} khóa nhà cung cấp đăng nhập ngoại vi", l.ProviderKey);
            }

            personalData.Add($"Khóa xác thực", await _userManager.GetAuthenticatorKeyAsync(user));

            Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
        }
    }
}
