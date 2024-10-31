using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.ViewComponents
{
    public class RightBarManagerViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string? role = "admin";
            if (role == "admin")
            {
                return View("AdminRightBar");
            } 
            return View("StaffRightBar");
        }
    }
}
