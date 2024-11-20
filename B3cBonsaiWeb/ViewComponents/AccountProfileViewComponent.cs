using B3cBonsai.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.ViewComponents
{
    public class AccountProfileViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        public AccountProfileViewComponent(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
        {
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
