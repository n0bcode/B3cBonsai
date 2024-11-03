﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.Utility
{
    public class SD
    {
        //Vai trò đăng nhập
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Staff = "Staff";

        //Tình trạng đơn hàng
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        //Tình trạng thanh toán
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

        //Loại đối tượng yêu thích cho "DanhSachYeuThich"
        public const string ObjectLike_SanPham = "SanPham";
        public const string ObjectLike_Comment = "Comment";

        //Loại đối tượng thanh toán cho "ChiTietDonHang" 
        public const string ObjectDetailOrder_SanPham = "SanPham";
        public const string ObjectDetailOrder_Combo = "Combo";

        //Tên Session chứa dữ liệu giỏ hàng
        public const string SessionCart = "SessionShoppingCart";
    }
}
