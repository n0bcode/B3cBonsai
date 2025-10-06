using System.Security.Claims;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.ViewComponents
{
    public class UserNotificationViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserNotificationViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return View(new List<ThongBao>()); // Empty list for non-authenticated users
            }

            // Get 5 most recent unread notifications
            var allNotifications = await _unitOfWork.ThongBao.GetAll(
                filter: n => n.NguoiDungId == userId && !n.DaDoc);
            var notifications = allNotifications
                .OrderByDescending(x => x.NgayTao)
                .Take(5)
                .ToList();

            return View(notifications);
        }
    }
}
