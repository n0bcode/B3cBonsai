using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{
    // Bảng thông báo cho người dùng
    public class ThongBao
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "ID người dùng không được để trống.")]
        public string NguoiDungId { get; set; } = string.Empty;

        [ValidateNever]
        [ForeignKey("NguoiDungId")]
        public virtual NguoiDungUngDung? NguoiDungUngDung { get; set; }

        [Required(ErrorMessage = "Tiêu đề thông báo không được để trống.")]
        [StringLength(200, ErrorMessage = "Tiêu đề thông báo không được vượt quá 200 ký tự.")]
        public string TieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung thông báo không được để trống.")]
        [StringLength(1000, ErrorMessage = "Nội dung thông báo không được vượt quá 1000 ký tự.")]
        public string NoiDung { get; set; } = string.Empty;
        public DateTimeOffset? NgayDoc { get; set; } = DateTimeOffset.UtcNow;
        public bool DaDoc { get; set; } = false;

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

        [Required(ErrorMessage = "Loại thông báo không được để trống.")]
        [StringLength(50, ErrorMessage = "Loại thông báo không được vượt quá 50 ký tự.")]
        public string Loai { get; set; } = string.Empty; // Loại thông báo: CapNhatDonHang, PhanHoiBinhLuan

        // ID của entity liên quan, ví dụ: ID đơn hàng hoặc ID bình luận
        public int? LienKetId { get; set; }
    }
}
