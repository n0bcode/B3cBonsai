using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{
    // Bảng Danh Mục Diễn Đàn (Forum Category)
    public class DanhMucDienDan
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên Danh Mục")]
        public string TenDanhMuc { get; set; }

        [Display(Name = "Mô Tả")]
        [StringLength(255)]
        public string? MoTa { get; set; }

        [Required]
        [StringLength(100)]
        public string Slug { get; set; } // SEO friendly URL part

        [ValidateNever]
        public virtual ICollection<ChuDe> ChuDes { get; set; }
    }
}
