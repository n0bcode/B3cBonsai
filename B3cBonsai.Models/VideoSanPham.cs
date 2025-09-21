using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{


    // Bảng video sản phẩm
    public class VideoSanPham
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(54)]
        public string TenVideo { get; set; } = string.Empty;

        [Required]
        public string LinkVideo { get; set; } = string.Empty;

        public int SanPhamId { get; set; }

        [ValidateNever]
        [ForeignKey("SanPhamId")]
        public virtual SanPham? SanPham { get; set; } // Khai báo quan hệ với sản phẩm
    }

}
