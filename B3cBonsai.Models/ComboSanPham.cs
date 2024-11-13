using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace B3cBonsai.Models
{
    // Bảng combo sản phẩm
    public class ComboSanPham
    {
        [Key]
        [Display(Name = "ID Combo")]
        [ValidateNever]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên combo không được để trống.")]
        [StringLength(54, ErrorMessage = "Tên combo không được vượt quá 54 ký tự.")]
        [Display(Name = "Tên Combo")]
        public string TenCombo { get; set; }

        [Display(Name = "Mô Tả")]
        public string MoTa { get; set; }

        [Required(ErrorMessage = "Giảm giá không được để trống.")]
        [Range(1, 99, ErrorMessage = "Giảm giá phải từ 1 đến 99.")]
        [Display(Name = "Giảm Giá (%)")]
        public int GiamGia { get; set; } = 5;

        [Range(0, int.MaxValue, ErrorMessage = "Tổng giá không được âm.")]
        [Display(Name = "Tổng Giá")]
        public int TongGia { get; set; }

        [ValidateNever]
        [Display(Name = "Chi Tiết Combo")]
        public virtual ICollection<ChiTietCombo> ChiTietCombos { get; set; } // Khai báo quan hệ 1-n với chi tiết combo
    }
}
