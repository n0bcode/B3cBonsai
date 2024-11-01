using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{
    // Bảng sản phẩm
    public class SanPham
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(54)]
        public string TenSanPham { get; set; }

        public int DanhMucId { get; set; }

        public string MoTa { get; set; }

        [Range(0, int.MaxValue)]
        public int SoLuong { get; set; }

        [Range(0, int.MaxValue)]
        public int Gia { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgaySuaDoi { get; set; } = DateTime.Now;

        [Required]
        public bool TrangThai { get; set; }

        [ValidateNever]
        [ForeignKey("DanhMucId")]
        public virtual DanhMucSanPham DanhMuc { get; set; } // Khai báo quan hệ với danh mục
        [ValidateNever]
        public virtual ICollection<HinhAnhSanPham> HinhAnhs { get; set; } // Khai báo quan hệ 1-n với hình ảnh
        [ValidateNever]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } // Khai báo quan hệ 1-n với chi tiết đơn hàng
        [ValidateNever]
        public virtual ICollection<BinhLuan> BinhLuans { get; set; } // Khai báo quan hệ 1-n với bình luận
        [ValidateNever]
        public virtual ICollection<DanhSachYeuThich> DanhSachYeuThichs { get; set; } // Khai báo quan hệ 1-n với danh sách yêu thích
        [ValidateNever]
        public virtual ICollection<ChiTietCombo> ChiTietCombos { get; set; } // Khai báo quan hệ 1-n với chi tiết combo
        [ValidateNever]
        public virtual ICollection<VideoSanPham> Videos { get; set; } // Khai báo quan hệ 1-n với video
    }
}
