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

    // Bảng chi tiết combo sản phẩm
    public class ChiTietCombo
    {
        [Key]
        public int Id { get; set; }

        public int ComboId { get; set; }
        [Required]
        public int SanPhamId { get; set; }
        [Required]
        public int SoLuong {  get; set; }

        [ValidateNever]
        [ForeignKey("ComboId")]
        public virtual ComboSanPham Combo { get; set; } // Khai báo quan hệ với combo
        [ValidateNever]
        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; } // Khai báo quan hệ với sản phẩm
    }
}
