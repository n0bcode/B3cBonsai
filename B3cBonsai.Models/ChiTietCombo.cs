using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B3cBonsai.Models
{
    // Bảng chi tiết combo sản phẩm
    public class ChiTietCombo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "ID combo không được để trống.")]
        public int ComboId { get; set; }

        [Required(ErrorMessage = "ID sản phẩm không được để trống.")]
        public int SanPhamId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 1.")]
        public int SoLuong { get; set; }

        [ValidateNever]
        [ForeignKey("ComboId")]
        public virtual ComboSanPham Combo { get; set; } // Khai báo quan hệ với combo

        [ValidateNever]
        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; } // Khai báo quan hệ với sản phẩm
    }
}
