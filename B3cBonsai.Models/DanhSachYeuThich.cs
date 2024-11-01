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
    // Bảng danh sách yêu thích
    public class DanhSachYeuThich
    {
        [Key]
        public int Id { get; set; }

        public int? SanPhamId { get; set; }

        public int? BinhLuanId { get; set; }

        public string NguoiDungId { get; set; }

        [Required]
        [StringLength(10)]
        public string LoaiDoiTuong { get; set; } = "SAN_PHAM";

        [ValidateNever]
        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; } // Khai báo quan hệ với sản phẩm
        [ValidateNever]
        [ForeignKey("BinhLuanId")]
        public virtual BinhLuan BinhLuan { get; set; } // Khai báo quan hệ với bình luận
        [ValidateNever]
        [ForeignKey("NguoiDungId")]
        public virtual NguoiDungUngDung NguoiDungUngDung { get; set; } // Khai báo quan hệ với người dùng
    }
}
