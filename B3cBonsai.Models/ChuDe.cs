using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{
    // Bảng Chủ Đề Diễn Đàn (Forum Thread)
    public class ChuDe
    {
        [Key]
        public int Id { get; set; }

        [ValidateNever]
        public string NguoiDungId { get; set; }

        [Required]
        [Display(Name = "Danh Mục")]
        public int DanhMucDienDanId { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự.")]
        [Display(Name = "Tiêu Đề")]
        public string TieuDe { get; set; }

        [Display(Name = "Nội Dung")]
        public string NoiDung { get; set; } // Can contain HTML/Markdown

        [Display(Name = "Lượt Xem")]
        public int LuotXem { get; set; } = 0;

        [Display(Name = "Ngày Tạo")]
        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

        [ValidateNever]
        [StringLength(250)]
        public string Slug { get; set; } // SEO friendly URL part

        [Display(Name = "Hình Ảnh")]
        [ValidateNever]
        public string LinkAnh { get; set; } // Stores multiple image links separated by semicolon

        [NotMapped]
        public List<Microsoft.AspNetCore.Http.IFormFile> ImageFiles { get; set; }

        [Display(Name = "Trạng Thái")]
        public bool TrangThai { get; set; } = true; // Open/Closed

        [ValidateNever]
        [ForeignKey("NguoiDungId")]
        public virtual NguoiDungUngDung NguoiDungUngDung { get; set; }

        [ValidateNever]
        [ForeignKey("DanhMucDienDanId")]
        public virtual DanhMucDienDan DanhMucDienDan { get; set; }

        [ValidateNever]
        public virtual ICollection<BaiViet> BaiViets { get; set; }
    }
}
