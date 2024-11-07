using B3cBonsai.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace B3cBonsai.Models
{
    // Bảng danh sách yêu thích
    public class DanhSachYeuThich
    {
        [Key]
        [Display(Name = "ID Danh Sách Yêu Thích")]
        public int Id { get; set; }

        [Display(Name = "ID Sản Phẩm")]
        public int? SanPhamId { get; set; }

        [Display(Name = "ID Bình Luận")]
        public int? BinhLuanId { get; set; }

        [Required(ErrorMessage = "ID người dùng không được để trống.")]
        [Display(Name = "ID Người Dùng")]
        public string NguoiDungId { get; set; }

        [Required(ErrorMessage = "Loại đối tượng không được để trống.")]
        [StringLength(10, ErrorMessage = "Loại đối tượng không được vượt quá 10 ký tự.")]
        [Display(Name = "Loại Đối Tượng")]
        public string LoaiDoiTuong { get; set; } = SD.ObjectLike_SanPham;

        [ValidateNever]
        [ForeignKey("SanPhamId")]
        [Display(Name = "Sản Phẩm")]
        public virtual SanPham SanPham { get; set; } // Khai báo quan hệ với sản phẩm

        [ValidateNever]
        [ForeignKey("BinhLuanId")]
        [Display(Name = "Bình Luận")]
        public virtual BinhLuan BinhLuan { get; set; } // Khai báo quan hệ với bình luận

        [ValidateNever]
        [ForeignKey("NguoiDungId")]
        [Display(Name = "Người Dùng")]
        public virtual NguoiDungUngDung NguoiDungUngDung { get; set; } // Khai báo quan hệ với người dùng
    }
}
