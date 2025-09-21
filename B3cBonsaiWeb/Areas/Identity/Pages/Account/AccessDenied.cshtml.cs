// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using B3cBonsai.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace B3cBonsaiWeb.Areas.Identity.Pages.Account
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class AccessDeniedModel : PageModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public void OnGet()
        {
            // Lấy quyền truy cập từ session, nếu không có thì gán quyền mặc định
            string viewAccess = HttpContext.Session.GetString(SD.ViewAccess) ?? SD.CustomerAccess;

            // Lưu quyền truy cập vào session
            HttpContext.Session.SetString(SD.ViewAccess, viewAccess);
        }
    }
}
