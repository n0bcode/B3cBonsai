using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility.Services;

namespace B3cBonsai.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public IBinhLuanRepository BinhLuan { get; private set; }

        public IChiTietComboRepository ChiTietCombo { get; private set; }

        public IChiTietDonHangRepository ChiTietDonHang { get; private set; }

        public IComboSanPhamRepository ComboSanPham { get; private set; }

        public IDanhMucSanPhamRepository DanhMucSanPham { get; private set; }

        public IDanhSachYeuThichRepository DanhSachYeuThich { get; private set; }

        public IDonHangRepository DonHang { get; private set; }

        public IHinhAnhSanPhamRepository HinhAnhSanPham { get; private set; }

        public INguoiDungUngDungRepository NguoiDungUngDung { get; private set; }

        public ISanPhamRepository SanPham { get; private set; }

        public IVideoSanPhamRepository VideoSanPham { get; private set; }
        public UnitOfWork(ApplicationDbContext db, IImageStorageService imageStorageService)
        {
            _db = db;
            BinhLuan = new BinhLuanRepository(_db);
            ChiTietCombo = new ChiTietComboRepository(_db);
            ChiTietDonHang = new ChiTietDonHangRepository(_db);
            ComboSanPham = new ComboSanPhamRepository(_db);
            DanhMucSanPham = new DanhMucSanPhamRepository(_db);
            DanhSachYeuThich = new DanhSachYeuThichRepository(_db);
            DonHang = new DonHangRepository(_db);
            HinhAnhSanPham = new HinhAnhSanPhamRepository(_db);
            NguoiDungUngDung = new NguoiDungUngDungRepository(_db, imageStorageService);
            SanPham = new SanPhamRepository(_db);
            VideoSanPham = new VideoSanPhamRepository(_db);

        }


        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
