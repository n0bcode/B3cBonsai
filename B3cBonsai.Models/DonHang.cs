using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.Models
{
    // Bảng đơn hàng
    public class DonHang
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NguoiDungId { get; set; }

        [ForeignKey("NguoiDungId")]
        [ValidateNever]
        public virtual NguoiDungUngDung NguoiDungUngDung { get; set; } // Khai báo quan hệ với người dùng

        public DateTime NgayDatHang { get; set; }
        public DateTime? NgayNhanHang { get; set; }
        public double TongTienDonHang { get; set; }

        public string? TrangThaiDonHang { get; set; } // OrderStatus từ `OrderHeader`
        public string? TrangThaiThanhToan { get; set; } // PaymentStatus từ `OrderHeader`
        public string? SoTheoDoi { get; set; } // TrackingNumber từ `OrderHeader`
        public string? NhaVanChuyen { get; set; } // Carrier từ `OrderHeader`

        public DateTime? NgayThanhToan { get; set; } // PaymentDate
        public DateTime? NgayHetHanThanhToan { get; set; } // PaymentDueDate

        public string? MaPhienThanhToan { get; set; } // SessionId
        public string? MaYeuCauThanhToan { get; set; } // PaymentIntentId

        [Required]
        [StringLength(18)]
        public string SoDienThoai { get; set; } // PhoneNumber từ `OrderHeader`

        [Required]
        [StringLength(255)]
        public string Duong { get; set; } // StreetAddress từ `OrderHeader`

        [Required]
        [StringLength(100)]
        public string ThanhPho { get; set; } // City từ `OrderHeader`

        [Required]
        [StringLength(100)]
        public string Tinh { get; set; } // State từ `OrderHeader`

        [Required]
        [StringLength(20)]
        public string MaBuuDien { get; set; } // PostalCode từ `OrderHeader`

        [Required]
        [StringLength(54)]
        public string TenNguoiNhan { get; set; } // Name từ `OrderHeader`

        [ValidateNever]
        [ForeignKey("NhanVienId")]
        public virtual NguoiDungUngDung NhanVien { get; set; } // Khai báo quan hệ với nhân viên

        [ValidateNever]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } // Khai báo quan hệ 1-n với chi tiết đơn hàng
    }
}
