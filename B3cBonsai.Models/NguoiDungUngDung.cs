using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B3cBonsai.Models
{
    public class NguoiDungUngDung : IdentityUser
    {
        [Required]
        [StringLength(54)]
        public string HoTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        [StringLength(18)]
        public string? SoDienThoai { get => PhoneNumber; set => _ = PhoneNumber; }
        public bool? GioiTinh { get; set; }

        [StringLength(18)]
        public string? CCCD { get; set; } // Số CCCD cần unique

        [StringLength(1024)]
        public string DiaChi { get; set; }

        [StringLength(255)]
        [ValidateNever]
        public string? LinkAnh { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public string? MoTa { get; set; }
        [ValidateNever]
        public virtual ICollection<DonHang> DonHangs { get; set; }

        [ValidateNever]
        public virtual ICollection<BinhLuan> BinhLuans { get; set; }

        [ValidateNever]
        public virtual ICollection<DanhSachYeuThich> DanhSachYeuThichs { get; set; }
        [NotMapped]
        public string? VaiTro {  get; set; }
    }
}
