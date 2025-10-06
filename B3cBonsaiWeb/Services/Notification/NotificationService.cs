using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;

namespace B3cBonsaiWeb.Services.Notification
{
    public class NotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Tạo thông báo cho người dùng khi đơn hàng được cập nhật
        /// </summary>
        public async Task CreateOrderUpdateNotification(string userId, string orderStatus, int orderId)
        {
            var title = "Cập nhật đơn hàng";
            var message = $"Đơn hàng của bạn đã được chuyển sang trạng thái: {GetOrderStatusDisplayName(orderStatus)}";

            var notification = new ThongBao
            {
                NguoiDungId = userId,
                TieuDe = title,
                NoiDung = message,
                Loai = SD.NotificationType_CapNhatDonHang,
                LienKetId = orderId
            };

            _unitOfWork.ThongBao.Add(notification);
            _unitOfWork.Save();
        }

        /// <summary>
        /// Tạo thông báo cho người dùng khi có bình luận được phản hồi
        /// </summary>
        public async Task CreateCommentReplyNotification(string userId, string replierName, int commentId)
        {
            var title = "Phản hồi bình luận";
            var message = $"{replierName} đã phản hồi bình luận của bạn.";

            var notification = new ThongBao
            {
                NguoiDungId = userId,
                TieuDe = title,
                NoiDung = message,
                Loai = SD.NotificationType_PhanHoiBinhLuan,
                LienKetId = commentId
            };

            _unitOfWork.ThongBao.Add(notification);
            _unitOfWork.Save();
        }

        private string GetOrderStatusDisplayName(string status)
        {
            return SD.OrderStatusDictionary.ContainsKey(status) ? SD.OrderStatusDictionary[status] : status;
        }
    }
}
