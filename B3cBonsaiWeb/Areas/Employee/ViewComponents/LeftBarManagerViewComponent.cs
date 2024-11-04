using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.ViewComponents
{
    public class LeftBarManagerViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Dữ liệu tĩnh
            string? role = "admin";
            if (role == "admin")
            {
                return View("AdminRightBar");
            } 
            return View("StaffRightBar");
        }
    }
}
