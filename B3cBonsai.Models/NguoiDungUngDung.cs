using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{
    public class NguoiDungUngDung : IdentityUser
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(54, ErrorMessage = "Họ tên không được vượt quá 54 ký tự.")]
        [Display(Name = "Họ Tên")]
        [RegularExpression(SD.ValidateStringName, ErrorMessage = "Họ tên chỉ được chứa chữ cái và khoảng trắng.")]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Ngày Sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(18, ErrorMessage = "Số điện thoại không được vượt quá 18 ký tự.")]
        [Display(Name = "Số Điện Thoại")]
        [RegularExpression(@"^\+?\d{1,3}?[-.\s]?\(?\d{1,3}\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? SoDienThoai { get => PhoneNumber; set => PhoneNumber = value; }

        [Display(Name = "Giới Tính")]
        public bool? GioiTinh { get; set; }

        [StringLength(18, ErrorMessage = "CCCD không được vượt quá 18 ký tự.", MinimumLength = 12)]
        [Display(Name = "Số CCCD")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số CCCD chỉ được chứa số.")]
        public string? CCCD { get; set; } // Số CCCD cần unique

        [StringLength(1024, ErrorMessage = "Địa chỉ không được vượt quá 1024 ký tự.")]
        [Display(Name = "Địa Chỉ")]
        [RegularExpression(SD.ValidateString, ErrorMessage = "Địa chỉ chỉ được chứa chữ cái, số và khoảng trắng.")]
        [Required(ErrorMessage = "Vui lòng nhập thông tin địa chỉ người dùng.")]
        public string DiaChi { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Link ảnh không được vượt quá 255 ký tự.")]
        [ValidateNever]
        [Display(Name = "Link Ảnh")]
        public string? LinkAnh { get; set; }

        [Display(Name = "Ngày Tạo")]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        [Display(Name = "Mô Tả")]
        [RegularExpression(SD.ValidateString, ErrorMessage = "Mô tả chỉ được chứa chữ cái, số và khoảng trắng.")]
        public string? MoTa { get; set; }


        [ValidateNever]
        public virtual ICollection<DonHang>? DonHangs { get; set; }

        [ValidateNever]
        public virtual ICollection<BinhLuan>? BinhLuans { get; set; }

        [ValidateNever]
        public virtual ICollection<DanhSachYeuThich>? DanhSachYeuThichs { get; set; }

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
        public string DatMatKhau { get; set; } = string.Empty;
    }
}
