using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.ViewComponents
{
    //Phần điều hướng phía trên bên phải của trang tùy theo tình trạng và role người dùng
    public class RightHeaderNavigationViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string? roleUser = "customer"; //Sau đó thay thành mã dùng kiểm tra vai trò người dùng đăng nhập
            if (roleUser != null)
            {
                if (roleUser == "admin" || roleUser == "staff") { 
                    return View("HeaderOfEmployee"); //Người dùng là nhân viên
                } else if (roleUser == "customer")
                {
                    return View("HeaderOfCustomer"); //Người dùng là khách hàng
                }
            }
            return View(); //Nếu người dùng chưa đăng nhập
        }
    }
}
