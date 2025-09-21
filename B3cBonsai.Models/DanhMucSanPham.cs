using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{
    // Bảng danh mục sản phẩm
    public class DanhMucSanPham
    {
        [Key]
        [Display(Name = "ID Danh Mục")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống.")]
        [StringLength(50, ErrorMessage = "Tên danh mục không được vượt quá 50 ký tự.")]
        [Display(Name = "Tên Danh Mục")]
        public string TenDanhMuc { get; set; } = string.Empty;

        [ValidateNever]
        [Display(Name = "Sản Phẩm")]
        public virtual ICollection<SanPham>? SanPhams { get; set; } = []; // Khai báo quan hệ 1-n với sản phẩm
    }
}
