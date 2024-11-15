﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B3cBonsai.Models
{
    public class NguoiDungUngDung : IdentityUser
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(54, ErrorMessage = "Họ tên không được vượt quá 54 ký tự.")]
        [Display(Name = "Họ Tên")]
        public string HoTen { get; set; }

        [Display(Name = "Ngày Sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(18, ErrorMessage = "Số điện thoại không được vượt quá 18 ký tự.")]
        [Display(Name = "Số Điện Thoại")]
        public string? SoDienThoai { get => PhoneNumber; set => PhoneNumber = value; }

        [Display(Name = "Giới Tính")]
        public bool? GioiTinh { get; set; }

        [StringLength(18, ErrorMessage = "CCCD không được vượt quá 18 ký tự.")]
        [Display(Name = "Số CCCD")]
        public string? CCCD { get; set; } // Số CCCD cần unique

        [StringLength(1024, ErrorMessage = "Địa chỉ không được vượt quá 1024 ký tự.")]
        [Display(Name = "Địa Chỉ")]
        public string DiaChi { get; set; }

        [StringLength(255, ErrorMessage = "Link ảnh không được vượt quá 255 ký tự.")]
        [ValidateNever]
        [Display(Name = "Link Ảnh")]
        public string? LinkAnh { get; set; }

        [Display(Name = "Ngày Tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Mô Tả")]
        public string? MoTa { get; set; }

        [ValidateNever]
        public virtual ICollection<DonHang> DonHangs { get; set; }

        [ValidateNever]
        public virtual ICollection<BinhLuan> BinhLuans { get; set; }

        [ValidateNever]
        public virtual ICollection<DanhSachYeuThich> DanhSachYeuThichs { get; set; }

        [NotMapped]
        [Display(Name = "Vai Trò")]
        public string? VaiTro { get; set; }
        [NotMapped]
        public bool ThaoTac { get; set; } = true; //Để xác định tạo hay không trong ManagerUser: true-Thêm // false-Sửa
        [NotMapped]
        [ValidateNever]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có từ 6 đến 100 ký tự.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{6,100}$",
        ErrorMessage = "Mật khẩu phải chứa ít nhất 1 ký tự số, 1 ký tự chữ thường, 1 ký tự chữ hoa và 1 ký tự đặc biệt.")]
        public string DatMatKhau { get; set; }
    }
}
