using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.ViewComponents
{
    public class RightHeaderProfileViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public RightHeaderProfileViewComponent(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork) {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
            return View(await _unitOfWork.NguoiDungUngDung.Get(x => x.Id == currentUser.Id));
        }
    }
}
