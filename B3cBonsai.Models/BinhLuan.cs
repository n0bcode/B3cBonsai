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
    // Bảng bình luận
    public class BinhLuan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string NoiDungBinhLuan { get; set; }

        public string NguoiDungId { get; set; }

        public int SanPhamId { get; set; }
        public bool TinhTrang { get; set; } = true;
        [ValidateNever]
        [ForeignKey("NguoiDungId")]
        public virtual NguoiDungUngDung NguoiDungUngDung { get; set; } // Khai báo quan hệ với người dùng
        [ValidateNever]
        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; } // Khai báo quan hệ với sản phẩm
    }
}
