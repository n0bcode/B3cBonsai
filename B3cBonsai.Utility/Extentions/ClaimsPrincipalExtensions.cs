using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;

namespace B3cBonsai.Utility.Extentions
{
    public static class ClaimsPrincipalExtensions
    {
        // Hàm này trả về thông tin người dùng đã đăng nhập dưới dạng IdentityUser
        public static IdentityUser GetLoggedInUser(this ClaimsPrincipal principal, UserManager<IdentityUser> userManager)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));
            if (userManager == null) throw new ArgumentNullException(nameof(userManager));

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return null;

            return userManager.FindByIdAsync(userId).Result; // Lưu ý: Nên sử dụng await
        }

        // Hàm này để lấy ID của người dùng đã đăng nhập
        public static T GetLoggedInUserId<T>(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var loggedInUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedInUserId == null)
                return default;

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(loggedInUserId, typeof(T));
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
            {
                return (T)Convert.ChangeType(loggedInUserId, typeof(T));
            }
            else
            {
                throw new Exception("Invalid type provided");
            }
        }

        // Hàm để lấy tên người dùng
        public static string GetLoggedInUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue(ClaimTypes.Name);
        }

        // Hàm để lấy email của người dùng
        public static string GetLoggedInUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue(ClaimTypes.Email);
        }
    }
}
