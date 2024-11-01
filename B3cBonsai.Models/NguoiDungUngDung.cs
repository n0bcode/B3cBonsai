using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B3cBonsai.Models
{
    public class NguoiDungUngDung
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(54)]
        public string HoTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        [StringLength(18)]
        public string SoDienThoai { get; set; }

        [StringLength(18)]
        public string? CCCD { get; set; } // Số CCCD cần unique

        [Required]
        [StringLength(54)]
        public string Email { get; set; } // Email cần unique

        [StringLength(1024)]
        public string DiaChi { get; set; }

        [Required]
        [StringLength(10)]
        public string VaiTro { get; set; } // Vai trò: 'KhachHang', 'Admin', 'NhanVien'

        [Required]
        [StringLength(18)]
        public string TenDangNhap { get; set; } // Tên đăng nhập cần unique

        [Required]
        [StringLength(255)]
        public string MatKhau { get; set; }

        [Required]
        public bool TrangThai { get; set; }

        [StringLength(255)]
        public string LinkAnh { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Required]
        public bool XacThucEmail { get; set; } = false;


        [ValidateNever]
        public virtual ICollection<DonHang> DonHangs { get; set; }

        [ValidateNever]
        public virtual ICollection<BinhLuan> BinhLuans { get; set; }

        [ValidateNever]
        public virtual ICollection<DanhSachYeuThich> DanhSachYeuThichs { get; set; }
    }
}
