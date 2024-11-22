using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.Models.ViewModels
{
    public class SanPhamOrComboVM
    {
        public SanPham? SanPham { get; set; }
        public ComboSanPham? ComboSanPham { get; set;}
        public string LoaiDoiTuong { get; set; }
    }
}
