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
    // Bảng combo sản phẩm
    public class ComboSanPham
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(54)]
        public string TenCombo { get; set; }

        public string MoTa { get; set; }
        [Required]
        [Range(1, 99)]
        public int GiamGia { get; set; } = 5;

        [Range(0, int.MaxValue)]
        public int TongGia { get; set; }    

        [ValidateNever]
        public virtual ICollection<ChiTietCombo> ChiTietCombos { get; set; } // Khai báo quan hệ 1-n với chi tiết combo
    }
}
