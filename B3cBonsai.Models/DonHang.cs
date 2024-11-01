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

        public string NguoiDungId { get; set; }

        public string? NhanVienId { get; set; }

        public DateTime NgayDatHang { get; set; } = DateTime.Now;

        [Required]
        [StringLength(10)]
        public string TrangThai { get; set; }

        [Required]
        [StringLength(5)]
        public string PhuongThucThanhToan { get; set; } //The / Tien

        public DateTime? NgayNhanHang { get; set; }

        public string? LyDoHuy { get; set; }

        [StringLength(54)]
        public string TenNguoiNhan { get; set; }

        [StringLength(18)]
        public string SoDienThoai { get; set; }

        [StringLength(1024)]
        public string DiaChi { get; set; }

        [ValidateNever]
        [ForeignKey("NguoiDungId")]
        public virtual NguoiDungUngDung NguoiDungUngDung { get; set; } // Khai báo quan hệ với người dùng
        [ValidateNever]
        [ForeignKey("NhanVienId")]
        public virtual NguoiDungUngDung NhanVien { get; set; } // Khai báo quan hệ với nhân viên
        [ValidateNever]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } // Khai báo quan hệ 1-n với chi tiết đơn hàng
    }
}
