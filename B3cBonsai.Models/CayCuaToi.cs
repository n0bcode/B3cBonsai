using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{
    // Bảng Cây Của Tôi (User's Tree)
    public class CayCuaToi
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Người dùng không được để trống.")]
        public string NguoiDungId { get; set; }

        [Display(Name = "Sản Phẩm Gốc")]
        public int? SanPhamId { get; set; } // Optional link to store product

        [Required(ErrorMessage = "Tên cây không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên cây không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên Cây (Biệt danh)")]
        public string TenCay { get; set; }

        [Display(Name = "Ngày Mua/Trồng")]
        public DateTimeOffset? NgayMua { get; set; }

        [Display(Name = "Trạng Thái")]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Bình thường"; // Healthy, Sick, Flowering, etc.

        [Display(Name = "Ghi Chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Hình Ảnh Đại Diện")]
        public string? HinhAnhDaiDien { get; set; }

        [Display(Name = "Ngày Tạo")]
        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

        [ValidateNever]
        [ForeignKey("NguoiDungId")]
        public virtual NguoiDungUngDung NguoiDungUngDung { get; set; }

        [ValidateNever]
        [ForeignKey("SanPhamId")]
        public virtual SanPham? SanPham { get; set; }

        [ValidateNever]
        public virtual ICollection<NhatKyCay> NhatKyCays { get; set; }
    }
}
