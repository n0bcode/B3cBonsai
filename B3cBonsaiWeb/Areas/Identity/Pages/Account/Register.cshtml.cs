// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace B3cBonsaiWeb.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Email là trường bắt buộc.")]
            [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Vui lòng nhập mặt khẩu cho tài khoản đang tạo.")]
            [StringLength(100, ErrorMessage = "Mặt khẩu phải có ít nhất {2} và nhiều nhất {1} độ dài kí tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            [RegularExpression(@"^(?=.*[A-Z]).*$", ErrorMessage = "Mật khẩu phải có ít nhất một ký tự viết hoa.")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
            public string ConfirmPassword { get; set; }

            [DataType(DataType.Text)]
            [Required(ErrorMessage = "Họ tên không được để trống.")]
            [StringLength(54, ErrorMessage = "Họ tên không được vượt quá 54 ký tự.")]
            [Display(Name = "Họ Tên")]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Họ tên chỉ được chứa chữ cái và khoảng trắng.")]
            public string HoTen {  get; set; }


            [Display(Name = "Giới tính")]
            public bool GioiTinh { get; set; }

            [StringLength(18, ErrorMessage = "Số điện thoại không được vượt quá 18 ký tự.")]
            [Display(Name = "Số Điện Thoại")]
            [RegularExpression(@"^\+?\d{1,3}?[-.\s]?\(?\d{1,3}\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
            public string SoDienThoai { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Ngày sinh")]
            public DateTime NgaySinh { get; set; }

            public string? Role { get; set; }
            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null, string? viewAccess = SD.CustomerAccess)
        {
            HttpContext.Session.SetString(SD.ViewAccess, viewAccess);

            Input = new()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.HoTen = Input.HoTen;
                user.GioiTinh = Input.GioiTinh;
                user.SoDienThoai = Input.SoDienThoai;
                user.NgaySinh = Input.NgaySinh;

                var result = await _userManager.CreateAsync(user, Input.Password);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Lấy quyền truy cập từ session, nếu không có thì gán quyền mặc định
                    string viewAccess = HttpContext.Session.GetString(SD.ViewAccess) ?? SD.CustomerAccess;

                    if (viewAccess == SD.CustomerAccess)
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Staff);
                        user.LockoutEnabled = true;
                        user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    #region//Tính năng chưa phát triển
                    /*var PathToFile = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                            + "Templates" + Path.DirectorySeparatorChar.ToString() + "EmailTemplates"
                            + Path.DirectorySeparatorChar.ToString() + "Confirm_Account_Registration.html";

                    var subject = "Confirm account registration";
                    string HtmlBody = "";
                    using (StreamReader streamReader = System.IO.File.OpenText(PathToFile))
                    {
                        HtmlBody = streamReader.ReadToEnd();
                    }

                    //{0} : Subject
                    //{1} : DateTime
                    //{2} : Name
                    //{3} : Email
                    //{4} : Message
                    //{5} : CallBackURL


                    string Message = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

                    string messageBody = string.Format(HtmlBody,
                        subject,
                        String.Format("{0:ddd, d MMMM yyyy}", DateTime.UtcNow),
                        user.UserName,
                        user.Email,
                        Message,
                        callbackUrl
                        );*/
                    #endregion

                    await _emailSender.SendEmailAsync(Input.Email, "Xác nhận email", $"Vui lòng xác nhận tài khoản của bạn <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>nhấn vào đây</a>.");

                    TempData["successToastr"] = "Đăng ký thành công";
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            Input = new()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            return Page();
        }

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

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Giao diện mặc định yêu cầu kho người dùng hỗ trợ email.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
