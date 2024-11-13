using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.Models
{
    public class ProductImageViewModel
    {
        public string LinkAnh { get; set; }
    }

    public class ProductViewModel
    {
        public string TenSanPham { get; set; }
        public List<ProductImageViewModel> HinhAnhs { get; set; }
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int SoLuong { get; set; }
        public ProductViewModel SanPham { get; set; }
    }

    public class OrderViewModel
    {
        public int Id { get; set; }
        public string TenNguoiNhan { get; set; }
        public string SoTheoDoi { get; set; }
        public string TrangThaiDonHang { get; set; }
        public DateTime NgayDatHang { get; set; }
        public DateTime? NgayNhanHang { get; set; }
        public string TrangThaiThanhToan { get; set; }
        public double TongTienDonHang { get; set; }
        public List<OrderItemViewModel> ChiTietDonHangs { get; set; }
    }


}
