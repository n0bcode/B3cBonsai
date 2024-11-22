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
    public class GioHang
    {
        [Key]
        public int Id { get; set; }

        public int? MaSanPham { get; set; }
        [ForeignKey("MaSanPham")]
        [ValidateNever]
        public SanPham? SanPham { get; set; }
        [Range(1, 1000, ErrorMessage = "Vui lòng nhập số lương trong phạm vi từ 1 -> 1000")]

        public int? MaCombo { get; set; }
        [ForeignKey("MaCombo")]
        [ValidateNever]
        public ComboSanPham? ComboSanPham { get; set; }

        [Range(1, 1000, ErrorMessage = "Vui lòng nhập số lương trong phạm vi từ 1 -> 1000")]
        public int SoLuong { get; set; }

        public string MaKhachHang { get; set; }
        [ForeignKey("MaKhachHang")]
        [ValidateNever]
        public NguoiDungUngDung NguoiDungUngDung{ get; set; }

        public string LoaiDoiTuong { get; set; }
        public string? LinkAnh { get; set; }

        [NotMapped]
        public double Gia { get; set; }
    }
}
