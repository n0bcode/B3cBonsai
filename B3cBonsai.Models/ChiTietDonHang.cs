using B3cBonsai.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B3cBonsai.Models
{
    // Bảng chi tiết đơn hàng
    public class ChiTietDonHang
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "ID đơn hàng không được để trống.")]
        public int DonHangId { get; set; }

        public int? SanPhamId { get; set; }

        public int? ComboId { get; set; }

        [Required(ErrorMessage = "Loại đối tượng không được để trống.")]
        [StringLength(10, ErrorMessage = "Loại đối tượng không được vượt quá 10 ký tự.")]
        public string LoaiDoiTuong { get; set; } = SD.ObjectDetailOrder_SanPham;

        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 1.")]
        public int SoLuong { get; set; }

        [Required(ErrorMessage = "Giá không được để trống.")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0.")]
        public int Gia { get; set; }

        [ValidateNever]
        [ForeignKey("DonHangId")]
        public virtual DonHang DonHang { get; set; } // Khai báo quan hệ với đơn hàng

        [ValidateNever]
        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; } // Khai báo quan hệ với sản phẩm

        [ValidateNever]
        [ForeignKey("ComboId")]
        public virtual ComboSanPham Combo { get; set; } // Khai báo quan hệ với combo
    }
}
