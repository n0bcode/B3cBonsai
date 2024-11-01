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

    // Bảng hình ảnh sản phẩm
    public class HinhAnhSanPham
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string LinkAnh { get; set; }

        public int SanPhamId { get; set; }

        [ValidateNever]
        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; } // Khai báo quan hệ với sản phẩm
    }
}
