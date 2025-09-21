using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{
    // Bảng bình luận
    public class BinhLuan
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nội dung bình luận không được để trống.")]
        [StringLength(128, ErrorMessage = "Nội dung bình luận không được quá 128 ký tự.")]
        [Display(Name = "Nội dung bình luận")]
        public string NoiDungBinhLuan { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID người dùng không được để trống.")]
        public string NguoiDungId { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID sản phẩm không được để trống.")]
        public int SanPhamId { get; set; }
        public DateTimeOffset NgayBinhLuan { get; set; } = DateTimeOffset.UtcNow;
        public bool TinhTrang { get; set; } = true;

        [ValidateNever]
        [ForeignKey("NguoiDungId")]
        public virtual NguoiDungUngDung? NguoiDungUngDung { get; set; } // Khai báo quan hệ với người dùng

        [ValidateNever]
        [ForeignKey("SanPhamId")]
        public virtual SanPham? SanPham { get; set; } // Khai báo quan hệ với sản phẩm
    }
}
