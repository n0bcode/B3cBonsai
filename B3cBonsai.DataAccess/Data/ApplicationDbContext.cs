using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B3cBonsai.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace B3cBonsai.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DanhMucSanPham> DanhMucSanPhams { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<ComboSanPham> ComboSanPhams { get; set; }
        public DbSet<ChiTietCombo> ChiTietCombos { get; set; }
        public DbSet<NguoiDungUngDung> NguoiDungUngDungs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<HinhAnhSanPham> HinhAnhSanPhams { get; set; }
        public DbSet<VideoSanPham> VideoSanPhams { get; set; }
        public DbSet<DanhSachYeuThich> DanhSachYeuThichs { get; set; }
        public DbSet<BinhLuan> BinhLuans { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thiết lập quan hệ giữa DonHang và NguoiDung (1-nhiều)
            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.NguoiDungUngDung)
                .WithMany(u => u.DonHangs)
                .HasForeignKey(d => d.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh vòng lặp xóa

            // Thiết lập quan hệ giữa SanPham và DanhMucSanPham (1-nhiều)
            modelBuilder.Entity<SanPham>()
                .HasOne(sp => sp.DanhMuc)
                .WithMany(dm => dm.SanPhams)
                .HasForeignKey(sp => sp.DanhMucId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ giữa ChiTietCombo và ComboSanPham (nhiều-nhiều)
            modelBuilder.Entity<ChiTietCombo>()
                .HasOne(ct => ct.Combo)
                .WithMany(c => c.ChiTietCombos)
                .HasForeignKey(ct => ct.ComboId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChiTietCombo>()
                .HasOne(ct => ct.SanPham)
                .WithMany(sp => sp.ChiTietCombos)
                .HasForeignKey(ct => ct.SanPhamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập quan hệ giữa ChiTietDonHang và DonHang (1-nhiều)
            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(ct => ct.DonHang)
                .WithMany(dh => dh.ChiTietDonHangs)
                .HasForeignKey(ct => ct.DonHangId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập quan hệ giữa ChiTietDonHang và SanPham (1-nhiều)
            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(ct => ct.SanPham)
                .WithMany(sp => sp.ChiTietDonHangs)
                .HasForeignKey(ct => ct.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ giữa DanhSachYeuThich và NguoiDung (1-nhiều)
            modelBuilder.Entity<DanhSachYeuThich>()
                .HasOne(ds => ds.NguoiDungUngDung)
                .WithMany(nd => nd.DanhSachYeuThichs)
                .HasForeignKey(ds => ds.NguoiDungId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập quan hệ giữa DanhSachYeuThich và SanPham (1-nhiều)
            modelBuilder.Entity<DanhSachYeuThich>()
                .HasOne(ds => ds.SanPham)
                .WithMany(sp => sp.DanhSachYeuThichs)
                .HasForeignKey(ds => ds.SanPhamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập quan hệ giữa BinhLuan và NguoiDung (1-nhiều)
            modelBuilder.Entity<BinhLuan>()
                .HasOne(bl => bl.NguoiDungUngDung)
                .WithMany(nd => nd.BinhLuans)
                .HasForeignKey(bl => bl.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ giữa BinhLuan và SanPham (1-nhiều)
            modelBuilder.Entity<BinhLuan>()
                .HasOne(bl => bl.SanPham)
                .WithMany(sp => sp.BinhLuans)
                .HasForeignKey(bl => bl.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ giữa HinhAnhSanPham và SanPham (1-nhiều)
            modelBuilder.Entity<HinhAnhSanPham>()
                .HasOne(ha => ha.SanPham)
                .WithMany(sp => sp.HinhAnhs)
                .HasForeignKey(ha => ha.SanPhamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập quan hệ giữa VideoSanPham và SanPham (1-nhiều)
            modelBuilder.Entity<VideoSanPham>()
                .HasOne(vs => vs.SanPham)
                .WithMany(sp => sp.Videos)
                .HasForeignKey(vs => vs.SanPhamId)
                .OnDelete(DeleteBehavior.Cascade);

            #region //Vùng tạo dữ liệu
            // Dữ liệu khởi tạo cho NGUOI_DUNG
            /*var nguoiDungs = new List<NguoiDungUngDung> { };

            nguoiDungs.Add(
                    new NguoiDungUngDung
                    {
                        Id = Guid.NewGuid().ToString(),
                        HoTen = "Trần Lý",
                        TenDangNhap = "tlys123", // Tên đăng nhập trong Identity
                        SoDienThoai = "911", // Gán số điện thoại
                        DiaChi = "146 Tinh Vân",
                        Email = "tructtpk03625@gmail.com", // Địa chỉ email
                        MatKhau = "123123", // Gán mật khẩu (nên băm trước khi lưu trữ)
                        VaiTro = "Admin", // Gán vai trò cho người dùng
                        LinkAnh = "https://i.pinimg.com/control/564x/6a/9c/77/6a9c77e0b1c7e5571ea5b5a350af0248.jpg",
                        NgayTao = DateTime.Now,
                        NgaySinh = DateTime.Now.AddDays(-new Random().Next(5000, 7200)), // Ngày sinh ngẫu nhiên
                        TrangThai = true,
                        XacThucEmail = true
                    });

            for (int i = 0; i < 30; i++)
            {
                nguoiDungs.Add(
                        new NguoiDungUngDung
                        {
                            Id = Guid.NewGuid().ToString(),
                            HoTen = RandomData_DB.Instance.rdName(),
                            TenDangNhap = "RD" + new Random().Next(1, 10000).ToString(), // Tên đăng nhập trong Identity
                            SoDienThoai = "911", // Gán số điện thoại
                            DiaChi = RandomData_DB.Instance.rdAddress(),
                            Email = RandomData_DB.Instance._Email(), // Địa chỉ email
                            MatKhau = new Random().Next(100000, 9999999).ToString(), // Gán mật khẩu (nên băm trước khi lưu trữ)
                            VaiTro = new string[] { "KhachHang", "Admin", "NhanVien" }[new Random().Next(3)],
                            LinkAnh = RandomData_DB.Instance._UserImage(),
                            NgayTao = DateTime.Now,
                            NgaySinh = DateTime.Now.AddDays(-new Random().Next(5000, 7200)), // Ngày sinh ngẫu nhiên
                            TrangThai = true,
                            XacThucEmail = true
                        });
            }

            modelBuilder.Entity<NguoiDungUngDung>().HasData(nguoiDungs);

            // Dữ liệu khởi tạo cho DANH_MUC_SAN_PHAM
            var danhMucSanPhams = new List<DanhMucSanPham>
    {
        new DanhMucSanPham { Id = 1, TenDanhMuc = "Cây lá màu" },
        new DanhMucSanPham { Id = 2, TenDanhMuc = "Cây thân gỗ bonsai" },
        new DanhMucSanPham { Id = 3, TenDanhMuc = "Cây hoa cảnh" },
        new DanhMucSanPham { Id = 4, TenDanhMuc = "Cây xương rồng và cây mọng nước" },
        new DanhMucSanPham { Id = 5, TenDanhMuc = "Cây cảnh để bàn" },
        new DanhMucSanPham { Id = 6, TenDanhMuc = "Cây cảnh thủy sinh" },
        new DanhMucSanPham { Id = 7, TenDanhMuc = "Cây phong thủy" },
        new DanhMucSanPham { Id = 8, TenDanhMuc = "Cây leo và cây treo" }
    };
            modelBuilder.Entity<DanhMucSanPham>().HasData(danhMucSanPhams);

            // Dữ liệu khởi tạo cho SAN_PHAM
            var sanPhams = new List<SanPham>();
            for (int i = 1; i <= 50; i++)
            {
                sanPhams.Add(new SanPham
                {
                    Id = i,
                    TenSanPham = RandomData_DB.Instance.RandomProductName(),
                    DanhMucId = new Random().Next(1, 9), // Lựa chọn ngẫu nhiên danh mục từ 1 đến 8
                    MoTa = RandomData_DB.Instance.RandomProductDescription(),
                    SoLuong = new Random().Next(1, 100),
                    Gia = new Random().Next(10000, 1000000),
                    NgayTao = DateTime.Now,
                    TrangThai = true
                });
            }
            modelBuilder.Entity<SanPham>().HasData(sanPhams);

            // Dữ liệu khởi tạo cho COMBO_SAN_PHAM
            var comboSanPhams = new List<ComboSanPham>();
            for (int i = 1; i <= 20; i++)
            {
                comboSanPhams.Add(new ComboSanPham
                {
                    Id = i,
                    TenCombo = "Combo " + i,
                    MoTa = "Combo sản phẩm " + i,
                    TongGia = new Random().Next(100000, 2000000)
                });
            }
            modelBuilder.Entity<ComboSanPham>().HasData(comboSanPhams);

            // Dữ liệu khởi tạo cho CHI_TIET_COMBO
            var chiTietCombos = new List<ChiTietCombo>();
            for (int i = 1; i <= 50; i++)
            {
                chiTietCombos.Add(new ChiTietCombo
                {
                    Id = i,
                    ComboId = new Random().Next(1, 21), // Lựa chọn ngẫu nhiên combo
                    SanPhamId = new Random().Next(1, 51), // Lựa chọn ngẫu nhiên sản phẩm
                    SoLuong = new Random().Next(1, 10)
                });
            }
            modelBuilder.Entity<ChiTietCombo>().HasData(chiTietCombos);

            // Dữ liệu khởi tạo cho DON_HANG
            var donHangs = new List<DonHang>();
            for (int i = 1; i <= 30; i++)
            {
                donHangs.Add(new DonHang
                {
                    Id = i,
                    NguoiDungId = nguoiDungs[0].Id, // Sử dụng ID của người dùng admin
                    NhanVienId = nguoiDungs[0].Id,
                    NgayDatHang = DateTime.Now.AddDays(-new Random().Next(0, 30)),
                    TrangThai = new[] { "ChoXacNhan", "HoanTat", "DaHuy" }[new Random().Next(3)],
                    PhuongThucThanhToan = "Tien",
                    NgayNhanHang = DateTime.Now.AddDays(new Random().Next(1, 7)),
                    TenNguoiNhan = RandomData_DB.Instance.rdName(),
                    SoDienThoai = RandomData_DB.Instance.RandomPhone(),
                    DiaChi = RandomData_DB.Instance.rdAddress()
                });
            }
            modelBuilder.Entity<DonHang>().HasData(donHangs);

            // Dữ liệu khởi tạo cho HINH_ANH_SAN_PHAM
            var hinhAnhSanPhams = new List<HinhAnhSanPham>();
            foreach (var product in sanPhams)
            {
                for (int i = 1; i <= 3; i++)
                {
                    hinhAnhSanPhams.Add(new HinhAnhSanPham
                    {
                        Id = (product.Id - 1) * 3 + i, // Đảm bảo ID duy nhất
                        LinkAnh = RandomData_DB.Instance.RandomProductImage(),
                        SanPhamId = product.Id
                    });
                }
            }
            modelBuilder.Entity<HinhAnhSanPham>().HasData(hinhAnhSanPhams);

            // Dữ liệu khởi tạo cho VIDEO_SAN_PHAM
            var videoSanPhams = new List<VideoSanPham>();
            foreach (var product in sanPhams)
            {
                videoSanPhams.Add(new VideoSanPham
                {
                    Id = product.Id,
                    TenVideo = "Video cho " + product.TenSanPham,
                    LinkVideo = RandomData_DB.Instance.RandomVideoBonsai(),
                    SanPhamId = product.Id
                });
            }
            modelBuilder.Entity<VideoSanPham>().HasData(videoSanPhams);

            // Dữ liệu khởi tạo cho DANH_SACH_YEU_THICH
            var danhSachYeuThichs = new List<DanhSachYeuThich>();
            foreach (var user in nguoiDungs)
            {
                for (int i = 1; i <= 5; i++)
                {
                    danhSachYeuThichs.Add(new DanhSachYeuThich
                    {
                        Id = new Random().Next(1, 99999999), // Đảm bảo ID duy nhất
                        SanPhamId = sanPhams[new Random().Next(sanPhams.Count)].Id,
                        NguoiDungId = user.Id
                    });
                }
            }
            modelBuilder.Entity<DanhSachYeuThich>().HasData(danhSachYeuThichs);

            // Dữ liệu khởi tạo cho BINH_LUAN
            var binhLuans = new List<BinhLuan>();
            foreach (var user in nguoiDungs)
            {
                for (int i = 1; i <= 10; i++)
                {
                    binhLuans.Add(new BinhLuan
                    {
                        Id = new Random().Next(1, 9999999), // Đảm bảo ID duy nhất
                        NoiDungBinhLuan = "Bình luận " + i + " của " + user.HoTen,
                        NguoiDungId = user.Id,
                        SanPhamId = sanPhams[new Random().Next(sanPhams.Count)].Id
                    });
                }
            }
            modelBuilder.Entity<BinhLuan>().HasData(binhLuans);

            // Dữ liệu khởi tạo cho CHI_TIET_DON_HANG
            var chiTietDonHangs = new List<ChiTietDonHang>();
            foreach (var order in donHangs)
            {
                for (int i = 1; i <= 3; i++)
                {
                    chiTietDonHangs.Add(new ChiTietDonHang
                    {
                        Id = (order.Id - 1) * 3 + i, // Đảm bảo ID duy nhất
                        DonHangId = order.Id,
                        SanPhamId = sanPhams[new Random().Next(sanPhams.Count)].Id,
                        LoaiSanPham = "SAN_PHAM",
                        SoLuong = new Random().Next(1, 5),
                        Gia = new Random().Next(10000, 1000000)
                    });
                }
            }
            modelBuilder.Entity<ChiTietDonHang>().HasData(chiTietDonHangs);*/
#endregion
        }
    }
}
