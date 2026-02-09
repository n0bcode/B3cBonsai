using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace B3cBonsai.Models
{
    // Bảng Bài Viết Diễn Đàn (Forum Post/Reply)
    public class BaiViet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChuDeId { get; set; }

        [Required]
        public string NguoiDungId { get; set; }

        [Required(ErrorMessage = "Nội dung bài viết không được để trống.")]
        [Display(Name = "Nội Dung")]
        public string NoiDung { get; set; }

        [Display(Name = "Là Câu Trả Lời Đúng")]
        public bool LaCauTraLoiDung { get; set; } = false;

        [Display(Name = "Ngày Tạo")]
        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

        [ValidateNever]
        [ForeignKey("ChuDeId")]
        public virtual ChuDe ChuDe { get; set; }

        [ValidateNever]
        [ForeignKey("NguoiDungId")]
        public virtual NguoiDungUngDung NguoiDungUngDung { get; set; }
    }
}
