using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace B3cBonsai.Models
{

    // Bảng hình ảnh sản phẩm
    public class HinhAnhSanPham
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? LinkAnh { get; set; } = string.Empty;

        public int SanPhamId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        [ForeignKey("SanPhamId")]
        public virtual SanPham? SanPham { get; set; } // Khai báo quan hệ với sản phẩm
    }
}
