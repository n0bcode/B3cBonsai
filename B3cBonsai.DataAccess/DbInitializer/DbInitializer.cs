using B3cBonsai.DataAccess.Data;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {
            ApplyMigrations();
            CreateRolesAndAdminUser();

            _db.SaveChanges();
        }

        private void ApplyMigrations()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception) { /* Log exception if needed */ }
        }

        private void CreateRolesAndAdminUser()
        {
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Staff)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new NguoiDungUngDung
                {
                    UserName = "admin@dotnetmastery.com",
                    Email = "admin@dotnetmastery.com",
                    HoTen = "Bhrugen Patel",
                    PhoneNumber = "1112223333",
                    DiaChi = "test 123 Ave",
                    LinkAnh = "https://i.pinimg.com/control/564x/6a/9c/77/6a9c77e0b1c7e5571ea5b5a350af0248.jpg",
                }, "Admin123*@").GetAwaiter().GetResult();

                var user = _db.NguoiDungUngDungs.FirstOrDefault(u => u.Email == "admin@dotnetmastery.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();


                SeedUsers();
                SeedProductCategories();
                SeedProducts();
                SeedCombos();
                SeedComboDetails();
                SeedProductImages();
                SeedProductVideos();
                SeedOrders();
                SeedFavorites();
                SeedComments();
                SeedOrderDetails();
            }
        }

        private void SeedUsers()
        {
            Random rd = new Random();
            var nguoiDungs = new List<NguoiDungUngDung>();

            for (int i = 0; i < 30; i++)
            {
                string generatedUserName;
                string generatedEmail;

                do
                {
                    generatedUserName = "RD" + rd.Next(1, 10000);
                    generatedEmail = RandomData_DB.Instance._Email();
                }
                while (_db.Users.Any(u => u.UserName == generatedUserName || u.Email == generatedEmail));

                nguoiDungs.Add(new NguoiDungUngDung
                {
                    HoTen = RandomData_DB.Instance.rdName(),
                    UserName = generatedUserName,
                    SoDienThoai = "09" + rd.Next(100000, 9999999),
                    DiaChi = RandomData_DB.Instance.rdAddress(),
                    Email = generatedEmail,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(rd.Next(100000, 9999999).ToString()),
                    LinkAnh = RandomData_DB.Instance._UserImage(),
                    NgayTao = DateTime.Now,
                    NgaySinh = DateTime.Now.AddDays(-rd.Next(5000, 7200))
                });
            }

            foreach (var nguoiDung in nguoiDungs)
            {
                var result = _userManager.CreateAsync(nguoiDung).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    var role = new[] { SD.Role_Admin, SD.Role_Staff, SD.Role_Customer }[rd.Next(3)];
                    _userManager.AddToRoleAsync(nguoiDung, role).GetAwaiter().GetResult();
                }
            }
        }

        private void SeedProductCategories()
        {
            var danhMucSanPhams = new List<DanhMucSanPham>
            {
                new DanhMucSanPham { TenDanhMuc = "Cây lá màu" },
                new DanhMucSanPham { TenDanhMuc = "Cây thân gỗ bonsai" },
                // Add other categories here as needed...
            };
            _db.DanhMucSanPhams.AddRange(danhMucSanPhams);
            _db.SaveChanges();
        }

        private void SeedProducts()
        {
            Random random = new Random();
            var sanPhams = new List<SanPham>();
            for (int i = 1; i <= 50; i++)
            {
                sanPhams.Add(new SanPham
                {
                    TenSanPham = RandomData_DB.Instance.RandomProductName(),
                    DanhMucId = random.Next(1, 3), // Assuming 2 categories
                    MoTa = RandomData_DB.Instance.RandomProductDescription(),
                    SoLuong = random.Next(1, 100),
                    Gia = random.Next(10000, 1000000),
                    NgayTao = DateTime.Now,
                    TrangThai = true
                });
            }
            _db.SanPhams.AddRange(sanPhams);
            _db.SaveChanges();
        }

        private void SeedCombos()
        {
            var comboSanPhams = new List<ComboSanPham>();
            for (int i = 1; i <= 20; i++)
            {
                comboSanPhams.Add(new ComboSanPham
                {
                    TenCombo = "Combo " + i,
                    MoTa = "Combo sản phẩm " + i,
                    TongGia = new Random().Next(100000, 2000000)
                });
            }
            _db.ComboSanPhams.AddRange(comboSanPhams);
            _db.SaveChanges();
        }

        private void SeedComboDetails()
        {
            var chiTietCombos = new List<ChiTietCombo>();
            for (int i = 1; i <= 50; i++)
            {
                chiTietCombos.Add(new ChiTietCombo
                {
                    ComboId = new Random().Next(1, 21),
                    SanPhamId = new Random().Next(1, 51),
                    SoLuong = new Random().Next(1, 10)
                });
            }
            _db.ChiTietCombos.AddRange(chiTietCombos);
            _db.SaveChanges();
        }

        private void SeedProductImages()
        {
            var sanPhams = _db.SanPhams.ToList();
            var hinhAnhSanPhams = new List<HinhAnhSanPham>();
            foreach (var product in sanPhams)
            {
                for (int i = 1; i <= 3; i++)
                {
                    hinhAnhSanPhams.Add(new HinhAnhSanPham
                    {
                        LinkAnh = RandomData_DB.Instance.RandomImageBonsai(),
                        SanPhamId = product.Id
                    });
                }
            }
            _db.HinhAnhSanPhams.AddRange(hinhAnhSanPhams);
            _db.SaveChanges();
        }

        private void SeedProductVideos()
        {
            var sanPhams = _db.SanPhams.ToList();
            var videoSanPhams = new List<VideoSanPham>();
            foreach (var product in sanPhams)
            {
                videoSanPhams.Add(new VideoSanPham
                {
                    TenVideo = "Video cho " + product.TenSanPham,
                    LinkVideo = RandomData_DB.Instance.RandomVideoBonsai(),
                    SanPhamId = product.Id
                });
            }
            _db.VideoSanPhams.AddRange(videoSanPhams);
            _db.SaveChanges();
        }

        private void SeedOrders()
        {
            Random rd = new Random();
            var donHangs = new List<DonHang>();

            for (int i = 1; i <= 30; i++)
            {
                // Lấy danh sách người dùng
                var nguoiDungIds = _db.NguoiDungUngDungs.Select(u => u.Id).ToList();
                var nguoiDungId = nguoiDungIds[rd.Next(nguoiDungIds.Count)];

                donHangs.Add(new DonHang
                {
                    NguoiDungId = nguoiDungId,
                    NhanVienId = _db.NguoiDungUngDungs.First().Id, // Giữ nguyên
                    NgayDatHang = DateTime.Now.AddDays(-rd.Next(0, 30)),
                    TrangThaiDonHang = new[] { SD.StatusInProcess, SD.StatusPending, SD.StatusCancelled, SD.StatusShipped, SD.StatusApproved }[rd.Next(5)],
                    NgayNhanHang = DateTime.Now.AddDays(rd.Next(1, 7)),
                    TenNguoiNhan = RandomData_DB.Instance.rdName(),
                    SoDienThoai = RandomData_DB.Instance.RandomPhone(),
                    ThanhPho = RandomData_DB.Instance.rdAddress(),
                    Duong = RandomData_DB.Instance.rdAddress(),
                    Tinh = RandomData_DB.Instance.rdAddress(),
                    MaBuuDien = rd.Next(1000, 9999).ToString()
                });
            }
            _db.DonHangs.AddRange(donHangs);
            _db.SaveChanges();
        }

        private void SeedFavorites()
        {
            var sanPhams = _db.SanPhams.ToList();
            var danhSachYeuThichs = new List<DanhSachYeuThich>();
            foreach (var userRD in _db.NguoiDungUngDungs.ToList())
            {
                for (int i = 1; i <= 5; i++)
                {
                    danhSachYeuThichs.Add(new DanhSachYeuThich
                    {
                        SanPhamId = sanPhams[new Random().Next(sanPhams.Count)].Id,
                        NguoiDungId = userRD.Id
                    });
                }
            }
            _db.DanhSachYeuThichs.AddRange(danhSachYeuThichs);
            _db.SaveChanges();
        }

        private void SeedComments()
        {
            var sanPhams = _db.SanPhams.ToList();
            var binhLuans = new List<BinhLuan>();
            foreach (var userRD in _db.NguoiDungUngDungs.ToList())
            {
                for (int i = 1; i <= 10; i++)
                {
                    binhLuans.Add(new BinhLuan
                    {
                        NoiDungBinhLuan = "Bình luận " + i + " của " + userRD.HoTen,
                        NguoiDungId = userRD.Id,
                        SanPhamId = sanPhams[new Random().Next(sanPhams.Count)].Id
                    });
                }
            }
            _db.BinhLuans.AddRange(binhLuans);
            _db.SaveChanges();
        }

        private void SeedOrderDetails()
        {
            var chiTietDonHangs = new List<ChiTietDonHang>();
            var sanPhamIds = _db.SanPhams.Select(sp => sp.Id).ToList(); // Lấy danh sách ID sản phẩm
            var comboIds = _db.ComboSanPhams.Select(cbo => cbo.Id).ToList();

            foreach (var order in _db.DonHangs.ToList())
            {
                for (int i = 1; i <= 3; i++)
                {
                    chiTietDonHangs.Add(new ChiTietDonHang
                    {
                        DonHangId = order.Id,
                        SanPhamId = sanPhamIds[new Random().Next(sanPhamIds.Count)], // Chọn sản phẩm ngẫu nhiên từ danh sách ID
                        LoaiDoiTuong = SD.ObjectDetailOrder_SanPham,
                        SoLuong = new Random().Next(1, 5),
                        Gia = new Random().Next(10000, 1000000)
                    });
                }
                chiTietDonHangs.Add(new ChiTietDonHang
                {
                    DonHangId = order.Id,
                    ComboId = comboIds[new Random().Next(comboIds.Count)],
                    LoaiDoiTuong = SD.ObjectDetailOrder_Combo,
                    SoLuong = 3,
                    Gia = new Random().Next(10000, 1000000)
                });
            }
            _db.ChiTietDonHangs.AddRange(chiTietDonHangs);
            _db.SaveChanges();
        }

    }
}
