using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{
    // Bảng Nhật Ký Cây (Tree Journal)
    public class NhatKyCay
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CayCuaToiId { get; set; }

        [Display(Name = "Ngày Tạo")]
        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

        [Required(ErrorMessage = "Nội dung nhật ký không được để trống.")]
        [Display(Name = "Nội Dung")]
        public string NoiDung { get; set; }

        [Display(Name = "Hình Ảnh")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Giai Đoạn Phát Triển")]
        [StringLength(50)]
        public string? GiaiDoanPhatTrien { get; set; } // Sprouting, Growing, Blooming, etc.

        [ValidateNever]
        [ForeignKey("CayCuaToiId")]
        public virtual CayCuaToi CayCuaToi { get; set; }
    }
}
