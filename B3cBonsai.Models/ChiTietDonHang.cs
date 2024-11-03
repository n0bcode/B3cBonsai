using B3cBonsai.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.Models
{

    // Bảng chi tiết đơn hàng
    public class ChiTietDonHang
    {
        [Key]
        public int Id { get; set; }

        public int DonHangId { get; set; }

        public int? SanPhamId { get; set; }

        public int? ComboId { get; set; }

        [Required]
        [StringLength(10)]
        public string LoaiDoiTuong { get; set; } = SD.ObjectDetailOrder_SanPham;

        [Required]
        [Range(1, int.MaxValue)]
        public int SoLuong { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
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
