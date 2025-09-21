using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace B3cBonsai.Models
{
    // Bảng sản phẩm
    public class SanPham
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(54, ErrorMessage = "Tên sản phẩm không được vượt quá 54 ký tự.")]
        [Display(Name = "Tên Sản Phẩm")]
        [RegularExpression(SD.ValidateStringName, ErrorMessage = "Tên sản phẩm chỉ được chứa chữ cái, số và khoảng trắng.")]
        public string TenSanPham { get; set; }

        [Display(Name = "Danh Mục")]
        public int DanhMucId { get; set; }

        [Display(Name = "Mô Tả")]
        //[RegularExpression(SD.ValidateString, ErrorMessage = "Mô tả chỉ được chứa chữ cái, số và khoảng trắng.")]
        public string MoTa { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số nguyên không âm.")]
        [Display(Name = "Số Lượng")]
        public int SoLuong { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Giá phải là số nguyên không âm.")]
        [Display(Name = "Giá")]
        public int Gia { get; set; }

        [Display(Name = "Ngày Tạo")]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        [Display(Name = "Ngày Sửa Đổi")]
        public DateTime NgaySuaDoi { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        [Display(Name = "Trạng Thái")]
        public bool TrangThai { get; set; }

        [ValidateNever]
        [ForeignKey("DanhMucId")]
        public virtual DanhMucSanPham DanhMuc { get; set; } // Khai báo quan hệ với danh mục

        [ValidateNever]
        [JsonIgnore]
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
