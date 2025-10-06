using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IBinhLuanRepository BinhLuan { get; }
        IChiTietComboRepository ChiTietCombo { get; }
        IChiTietDonHangRepository ChiTietDonHang { get; }
        IComboSanPhamRepository ComboSanPham { get; }
        IDanhMucSanPhamRepository DanhMucSanPham { get; }
        IDanhSachYeuThichRepository DanhSachYeuThich { get; }
        IDonHangRepository DonHang { get; }
        IHinhAnhSanPhamRepository HinhAnhSanPham { get; }
        INguoiDungUngDungRepository NguoiDungUngDung { get; }
        ISanPhamRepository SanPham { get; }
        IVideoSanPhamRepository VideoSanPham { get; }
        IThongBaoRepository ThongBao { get; }
        void Save();
    }
}
