using B3cBonsai.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B3cBonsai.Models
{
    // Bảng đơn hàng
    public class DonHang
    {
        [Key]
        [Display(Name = "ID Đơn Hàng")]
        public int Id { get; set; }

        [Required(ErrorMessage = "ID người dùng không được để trống.")]
        [Display(Name = "ID Người Dùng")]
        public string NguoiDungId { get; set; }

        [ForeignKey("NguoiDungId")]
        [ValidateNever]
        [Display(Name = "Người Dùng")]
        public virtual NguoiDungUngDung NguoiDungUngDung { get; set; } // Khai báo quan hệ với người dùng

        [Display(Name = "ID Nhân Viên")]
        public string? NhanVienId { get; set; }

        [ValidateNever]
        [ForeignKey("NhanVienId")]
        [Display(Name = "Nhân Viên")]
        public virtual NguoiDungUngDung NhanVien { get; set; } // Khai báo quan hệ với nhân viên

        [Display(Name = "Ngày Đặt Hàng")]
        public DateTime NgayDatHang { get; set; }

        [Display(Name = "Ngày Nhận Hàng")]
        public DateTime? NgayNhanHang { get; set; }

        [Display(Name = "Tổng Tiền Đơn Hàng")]
        public double TongTienDonHang { get; set; }

        [Display(Name = "Trạng Thái Đơn Hàng")]
        public string? TrangThaiDonHang { get; set; } // OrderStatus từ `OrderHeader`

        [Display(Name = "Trạng Thái Thanh Toán")]
        public string? TrangThaiThanhToan { get; set; } = SD.PaymentStatusPending; // PaymentStatus từ `OrderHeader`

        [Display(Name = "Số Theo Dõi")]
        public string? SoTheoDoi { get; set; } // TrackingNumber từ `OrderHeader`

        [Display(Name = "Nhà Vận Chuyển")]
        public string? NhaVanChuyen { get; set; } // Carrier từ `OrderHeader`

        [Display(Name = "Ngày Thanh Toán")]
        public DateTime? NgayThanhToan { get; set; } // PaymentDate

        [Display(Name = "Ngày Hết Hạn Thanh Toán")]
        public DateTime? NgayHetHanThanhToan { get; set; } // PaymentDueDate

        [Display(Name = "Mã Phiên Thanh Toán")]
        public string? MaPhienThanhToan { get; set; } // SessionId

        [Display(Name = "Mã Yêu Cầu Thanh Toán")]
        public string? MaYeuCauThanhToan { get; set; } // PaymentIntentId

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [StringLength(18, ErrorMessage = "Số điện thoại không được vượt quá 18 ký tự.")]
        [Display(Name = "Số Điện Thoại")]
        public string SoDienThoai { get; set; } // PhoneNumber từ `OrderHeader`

        [Required(ErrorMessage = "Đường không được để trống.")]
        [StringLength(255, ErrorMessage = "Đường không được vượt quá 255 ký tự.")]
        [Display(Name = "Đường")]
        public string Duong { get; set; } // StreetAddress từ `OrderHeader`

        [Required(ErrorMessage = "Thành phố không được để trống.")]
        [StringLength(100, ErrorMessage = "Thành phố không được vượt quá 100 ký tự.")]
        [Display(Name = "Thành Phố")]
        public string ThanhPho { get; set; } // City từ `OrderHeader`

        [Required(ErrorMessage = "Tỉnh không được để trống.")]
        [StringLength(100, ErrorMessage = "Tỉnh không được vượt quá 100 ký tự.")]
        [Display(Name = "Tỉnh")]
        public string Tinh { get; set; } // State từ `OrderHeader`

        [Required(ErrorMessage = "Mã bưu điện không được để trống.")]
        [StringLength(20, ErrorMessage = "Mã bưu điện không được vượt quá 20 ký tự.")]
        [Display(Name = "Mã Bưu Điện")]
        public string MaBuuDien { get; set; } // PostalCode từ `OrderHeader`

        [Required(ErrorMessage = "Tên người nhận không được để trống.")]
        [StringLength(54, ErrorMessage = "Tên người nhận không được vượt quá 54 ký tự.")]
        [Display(Name = "Tên Người Nhận")]
        public string TenNguoiNhan { get; set; } // Name từ `OrderHeader`

        [ValidateNever]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } // Khai báo quan hệ 1-n với chi tiết đơn hàng
    }
}
