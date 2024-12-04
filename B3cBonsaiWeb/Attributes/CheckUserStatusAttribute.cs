using B3cBonsai.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace B3cBonsaiWeb.Attributes
{
    public class CheckUserStatusAttribute : ActionFilterAttribute
    {

        public async Task OnActionExecuting(AuthorizationFilterContext context)
        {
            var unitOfWork = context.HttpContext.RequestServices.GetService<IUnitOfWork>();
            Console.WriteLine("CheckUserStatusAttribute executed");

            if (unitOfWork == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Area", "Customer" },
                        {"Controller", "Home" },
                        {"Action", "Index" }
                    });
                return;
            }

            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra nếu không có thông tin người dùng
            if (userId == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Area", "Customer" },
                        {"Controller", "Home" },
                        {"Action", "Index" }
                    }); 
                return;
            }

            // Lấy thông tin người dùng từ cơ sở dữ liệu
            var user = await unitOfWork.NguoiDungUngDung.Get(nd => nd.Id == userId);

            // Kiểm tra xem người dùng có tồn tại không
            if (user == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Area", "Customer" },
                        {"Controller", "Home" },
                        {"Action", "Index" }
                    });
                return;
            }

            // Kiểm tra tài khoản có bị khóa hay không
            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Area", "Identity" },
                        {"Controller", "Account" },
                        {"Action", "AccessDenied" }
                    });
                return;
            }

            // Kiểm tra các thuộc tính khác nếu cần
            if (!user.EmailConfirmed)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Area", "Identity" },
                        {"Controller", "Account" },
                        {"Action", "Login" }
                    });

                //context.Result = new BadRequestObjectResult(new { message = "Tài khoản chưa được xác minh email." });
                return;
            }

            // Add any other checks as necessary
        }
    }
}
