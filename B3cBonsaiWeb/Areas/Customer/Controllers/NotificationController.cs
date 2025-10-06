using System.Security.Claims;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using B3cBonsaiWeb.Services.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationService _notificationService;

        public NotificationController(IUnitOfWork unitOfWork, NotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        // GET: Customer/Notification
        public async Task<IActionResult> Index(int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Get all notifications for the user
            var allNotifications = await _unitOfWork.ThongBao.GetAll(
                filter: n => n.NguoiDungId == userId);

            // Sort by creation date (most recent first)
            var orderedNotifications = allNotifications
                .OrderByDescending(x => x.NgayTao);

            // Convert to paged list
            var pagedNotifications = orderedNotifications.ToPagedList(pageNumber, pageSize);

            return View(pagedNotifications);
        }

        // GET: Customer/Notification/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thongBao = await _unitOfWork.ThongBao.Get(
                t => t.Id == id && t.NguoiDungId == userId);

            if (thongBao == null)
            {
                return NotFound();
            }

            // Mark as read if not already
            if (!thongBao.DaDoc)
            {
                thongBao.DaDoc = true;
                _unitOfWork.ThongBao.Update(thongBao);
                _unitOfWork.Save();
            }

            return View(thongBao);
        }

        // POST: Customer/Notification/MarkAsRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thongBao = await _unitOfWork.ThongBao.Get(
                t => t.Id == id && t.NguoiDungId == userId);

            if (thongBao == null)
            {
                return Json(new { success = false, message = "Thông báo không tồn tại" });
            }

            if (!thongBao.DaDoc)
            {
                thongBao.DaDoc = true;
                thongBao.NgayDoc = DateTimeOffset.Now;
                _unitOfWork.ThongBao.Update(thongBao);

                try
                {
                    _unitOfWork.Save();
                    return Json(new { success = true });
                }
                catch
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật thông báo" });
                }
            }

            return Json(new { success = true });
        }

        // POST: Customer/Notification/MarkAllAsRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var unreadNotifications = (await _unitOfWork.ThongBao.GetAll(
                    filter: n => n.NguoiDungId == userId && !n.DaDoc)).ToList();

                foreach (var notification in unreadNotifications)
                {
                    notification.DaDoc = true;
                    notification.NgayDoc = DateTimeOffset.Now;
                    _unitOfWork.ThongBao.Update(notification);
                }

                _unitOfWork.Save();

                return Json(new { success = true, markedCount = unreadNotifications.Count });
            }
            catch
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật thông báo" });
            }
        }

        // GET: Customer/Notification/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thongBao = await _unitOfWork.ThongBao.Get(
                t => t.Id == id && t.NguoiDungId == userId);

            if (thongBao == null)
            {
                return NotFound();
            }

            return View(thongBao);
        }

        // POST: Customer/Notification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thongBao = await _unitOfWork.ThongBao.Get(
                t => t.Id == id && t.NguoiDungId == userId);

            if (thongBao != null)
            {
                _unitOfWork.ThongBao.Remove(thongBao);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
