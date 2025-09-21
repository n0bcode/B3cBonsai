using B3cBonsai.Models;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.ViewComponents
{
    public class LeftBarManagerViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.IsInRole(SD.Role_Admin))
            {
                return View("AdminRightBar");
            }
            return View("StaffRightBar");
        }
    }
}
