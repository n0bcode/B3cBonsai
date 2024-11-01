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

    // Bảng danh mục sản phẩm
    public class DanhMucSanPham
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TenDanhMuc { get; set; }

        [ValidateNever]
        public virtual ICollection<SanPham> SanPhams { get; set; } // Khai báo quan hệ 1-n với sản phẩm
    }
}
