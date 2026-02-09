using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace B3cBonsai.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db, IConfiguration configuration, ILogger<DbInitializer> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _configuration = configuration;
            _logger = logger;
        }

        public void Initialize()
        {
            ApplyMigrations();
            CreateRolesAndAdminUser();
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
                    GioiTinh = true,
                    PhoneNumber = "1112223333",
                    DiaChi = "test 123 Ave",
                    LinkAnh = "https://i.pinimg.com/control/564x/6a/9c/77/6a9c77e0b1c7e5571ea5b5a350af0248.jpg",
                }, "Admin123*@").GetAwaiter().GetResult();

                var user = _db.NguoiDungUngDungs.FirstOrDefault(u => u.Email == "admin@dotnetmastery.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

                // Create regular test user for customer testing
                _userManager.CreateAsync(new NguoiDungUngDung
                {
                    UserName = "customer@dotnetmastery.com",
                    Email = "customer@dotnetmastery.com",
                    HoTen = "Nguyễn Văn Customer",
                    GioiTinh = true,
                    PhoneNumber = "0987654321",
                    DiaChi = "123 Đường ABC, Quận 1, TP.HCM",
                    LinkAnh = "https://i.pinimg.com/control/564x/6a/9c/77/6a9c77e0b1c7e5571ea5b5a350af0248.jpg",
                }, "Customer123*@").GetAwaiter().GetResult();

                var customerUser = _db.NguoiDungUngDungs.FirstOrDefault(u => u.Email == "customer@dotnetmastery.com");
                _userManager.AddToRoleAsync(customerUser, SD.Role_Customer).GetAwaiter().GetResult();

                SeedSampleData();
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
                    GioiTinh = rd.NextDouble() < 0.5,
                    MoTa = RandomData_DB.Instance._Descript(),
                    Email = generatedEmail,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(rd.Next(100000, 9999999).ToString()),
                    LinkAnh = RandomData_DB.Instance._UserImage(),
                    NgayTao = DateTime.UtcNow,
                    NgaySinh = DateTime.UtcNow.AddDays(-rd.Next(5000, 7200))
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
            if (_db.DanhMucSanPhams.Any())
            {
                return; // Already seeded
            }

            string[] categories = {
                "Cây lá màu",
                "Cây thân gỗ bonsai",
                "Cây hoa cảnh",
                "Cây xương rồng và cây mọng nước",
                "Cây cảnh để bàn",
                "Cây cảnh thủy sinh",
                "Cây phong thủy",
                "Cây leo và cây treo"
            };

            var danhMucSanPhams = new List<DanhMucSanPham>();
            foreach (var category in categories)
            {
                danhMucSanPhams.Add(new DanhMucSanPham
                {
                    TenDanhMuc = category
                });
            }

            _db.DanhMucSanPhams.AddRange(danhMucSanPhams);
            _db.SaveChanges();
        }

        private void SeedProducts()
        {
            if (_db.SanPhams.Any())
            {
                return; // Already seeded
            }

            Random random = new Random();
            var categoryIds = _db.DanhMucSanPhams.Select(c => c.Id).ToList();
            var sanPhams = new List<SanPham>();

            for (int i = 1; i <= 50; i++)
            {
                sanPhams.Add(new SanPham
                {
                    TenSanPham = RandomData_DB.Instance.RandomProductName(),
                    DanhMucId = categoryIds[random.Next(categoryIds.Count)],
                    MoTa = RandomData_DB.Instance.RandomProductDescription(),
                    SoLuong = random.Next(1, 10),
                    Gia = random.Next(10000, 1000000),
                    NgayTao = DateTime.UtcNow,
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
                    TongGia = new Random().Next(100000, 2000000),
                    LinkAnh = RandomData_DB.Instance.RandomImageBonsai()
                });
            }
            _db.ComboSanPhams.AddRange(comboSanPhams);
            _db.SaveChanges();
        }

        private void SeedComboDetails()
        {
            if (_db.ChiTietCombos.Any())
            {
                return; // Already seeded
            }

            Random random = new Random();
            var comboIds = _db.ComboSanPhams.Select(c => c.Id).ToList();
            var productIds = _db.SanPhams.Select(p => p.Id).ToList();

            var chiTietCombos = new List<ChiTietCombo>();
            for (int i = 1; i <= 50; i++)
            {
                chiTietCombos.Add(new ChiTietCombo
                {
                    ComboId = comboIds[random.Next(comboIds.Count)],
                    SanPhamId = productIds[random.Next(productIds.Count)],
                    SoLuong = random.Next(1, 10)
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

        private void SeedForumCategories()
        {
            if (_db.DanhMucDienDans.Any())
            {
                return; // Already seeded
            }

            var categories = new List<DanhMucDienDan>
            {
                new DanhMucDienDan { TenDanhMuc = "Hỏi Đáp Chung", Slug = "hoi-dap-chung", MoTa = "Thảo luận chung về cây cảnh" },
                new DanhMucDienDan { TenDanhMuc = "Kỹ Thuật Chăm Sóc", Slug = "ky-thuat-cham-soc", MoTa = "Chia sẻ kỹ thuật chăm sóc cây" },
                new DanhMucDienDan { TenDanhMuc = "Demo/Tạo Dáng", Slug = "demo-tao-dang", MoTa = "Khoe cây và tạo dáng" },
                new DanhMucDienDan { TenDanhMuc = "Mua Bán/Trao Đổi", Slug = "mua-ban-trao-doi", MoTa = "Giao lưu mua bán cây cảnh" }
            };

            _db.DanhMucDienDans.AddRange(categories);
            _db.SaveChanges();
        }

        private void SeedOrders()
        {
            Random rd = new Random();
            var donHangs = new List<DonHang>();

            for (int i = 1; i <= 200; i++)
            {
                // Lấy danh sách người dùng
                var nguoiDungIds = _db.NguoiDungUngDungs.Select(u => u.Id).ToList();
                var nguoiDungId = nguoiDungIds[rd.Next(nguoiDungIds.Count)];

                donHangs.Add(new DonHang
                {
                    NguoiDungId = nguoiDungId,
                    NhanVienId = _db.NguoiDungUngDungs.First().Id, // Giữ nguyên
                    NgayDatHang = DateTime.UtcNow.AddDays(-rd.Next(30, 365)),
                    TrangThaiDonHang = new[] { SD.StatusInProcess, SD.StatusPending, SD.StatusCancelled, SD.StatusShipped, SD.StatusApproved }[rd.Next(5)],
                    NgayNhanHang = DateTime.UtcNow.AddDays(rd.Next(0, 30)),
                    TenNguoiNhan = RandomData_DB.Instance.rdName(),
                    SoDienThoai = RandomData_DB.Instance.RandomPhone(),
                    ThanhPho = RandomData_DB.Instance.rdAddress(),
                    Duong = RandomData_DB.Instance.rdAddress(),
                    Tinh = RandomData_DB.Instance.rdAddress(),
                    MaBuuDien = rd.Next(1000, 9999).ToString(),
                    TongTienDonHang = rd.Next(20000, 50000)
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
            Random rd = new Random();
            foreach (var userRD in _db.NguoiDungUngDungs.ToList())
            {
                for (int i = 1; i <= 10; i++)
                {
                    binhLuans.Add(new BinhLuan
                    {
                        NoiDungBinhLuan = "Bình luận " + i + " của " + userRD.HoTen,
                        NguoiDungId = userRD.Id,
                        SanPhamId = sanPhams[new Random().Next(sanPhams.Count)].Id,
                        NgayBinhLuan = (DateTime.UtcNow.AddDays(-rd.Next(3, 10)))
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

        private void SeedSampleCustomerData(string customerId)
        {
            Random rd = new Random();

            // Seed sample orders for the customer
            var donHangs = new List<DonHang>();
            for (int i = 1; i <= 10; i++)
            {
                donHangs.Add(new DonHang
                {
                    NguoiDungId = customerId,
                    NhanVienId = _db.NguoiDungUngDungs.First().Id,
                    NgayDatHang = DateTime.UtcNow.AddDays(-rd.Next(30, 365)),
                    TrangThaiDonHang = new[] { SD.StatusInProcess, SD.StatusPending, SD.StatusCancelled, SD.StatusShipped, SD.StatusApproved }[rd.Next(5)],
                    NgayNhanHang = DateTime.UtcNow.AddDays(rd.Next(0, 30)),
                    TenNguoiNhan = RandomData_DB.Instance.rdName(),
                    SoDienThoai = RandomData_DB.Instance.RandomPhone(),
                    ThanhPho = RandomData_DB.Instance.rdAddress(),
                    Duong = RandomData_DB.Instance.rdAddress(),
                    Tinh = RandomData_DB.Instance.rdAddress(),
                    MaBuuDien = rd.Next(1000, 9999).ToString(),
                    TongTienDonHang = rd.Next(20000, 50000)
                });
            }
            _db.DonHangs.AddRange(donHangs);
            _db.SaveChanges();

            // Seed order details for the sample orders
            var chiTietDonHangs = new List<ChiTietDonHang>();
            var sanPhamIds = _db.SanPhams.Select(sp => sp.Id).ToList();
            var comboIds = _db.ComboSanPhams.Select(cbo => cbo.Id).ToList();

            foreach (var order in donHangs)
            {
                for (int i = 1; i <= 3; i++)
                {
                    chiTietDonHangs.Add(new ChiTietDonHang
                    {
                        DonHangId = order.Id,
                        SanPhamId = sanPhamIds[new Random().Next(sanPhamIds.Count)],
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

            // Seed sample favorites for the customer
            var sanPhams = _db.SanPhams.ToList();
            var danhSachYeuThichs = new List<DanhSachYeuThich>();
            for (int i = 1; i <= 8; i++)
            {
                danhSachYeuThichs.Add(new DanhSachYeuThich
                {
                    SanPhamId = sanPhams[new Random().Next(sanPhams.Count)].Id,
                    NguoiDungId = customerId
                });
            }
            _db.DanhSachYeuThichs.AddRange(danhSachYeuThichs);
            _db.SaveChanges();

            // Seed sample comments for the customer
            var binhLuans = new List<BinhLuan>();
            var customer = _db.NguoiDungUngDungs.FirstOrDefault(u => u.Id == customerId);
            for (int i = 1; i <= 15; i++)
            {
                binhLuans.Add(new BinhLuan
                {
                    NoiDungBinhLuan = "Sample comment " + i + " from " + customer.HoTen,
                    NguoiDungId = customerId,
                    SanPhamId = sanPhams[new Random().Next(sanPhams.Count)].Id,
                    NgayBinhLuan = (DateTime.UtcNow.AddDays(-rd.Next(3, 10)))
                });
            }
            _db.BinhLuans.AddRange(binhLuans);
            _db.SaveChanges();

            // Seed sample notifications for the customer
            var thongBaos = new List<ThongBao>();
            Random rdThongBao = new Random();

            // Notifications for orders
            foreach (var order in donHangs)
            {
                // Order placement notification
                thongBaos.Add(new ThongBao
                {
                    NguoiDungId = customerId,
                    TieuDe = "Đơn hàng của bạn đã được đặt thành công",
                    NoiDung = $"Đơn hàng #{order.Id} đã được tạo với tổng giá trị {order.TongTienDonHang} VND.",
                    DaDoc = rdThongBao.NextDouble() < 0.7, // 70% chance of being read
                    NgayTao = order.NgayDatHang,
                    Loai = "CapNhatDonHang",
                    LienKetId = order.Id
                });

                // Random status update notification
                if (rdThongBao.NextDouble() < 0.8) // 80% chance
                {
                    string statusMessage = order.TrangThaiDonHang switch
                    {
                        SD.StatusInProcess => "đang được xử lý",
                        SD.StatusPending => "đang chờ xác nhận",
                        SD.StatusCancelled => "đã bị hủy",
                        SD.StatusShipped => "đang được giao",
                        SD.StatusApproved => "đã được duyệt",
                        _ => "có cập nhật"
                    };

                    thongBaos.Add(new ThongBao
                    {
                        NguoiDungId = customerId,
                        TieuDe = "Cập nhật trạng thái đơn hàng",
                        NoiDung = $"Đơn hàng #{order.Id} của bạn hiện tại {statusMessage}.",
                        DaDoc = rdThongBao.NextDouble() < 0.5,
                        NgayTao = order.NgayDatHang.AddDays(rdThongBao.Next(1, 7)),
                        Loai = "CapNhatDonHang",
                        LienKetId = order.Id
                    });
                }
            }

            // Notifications for comments
            foreach (var binhLuan in binhLuans.Take(10)) // Sample notifications for some comments
            {
                thongBaos.Add(new ThongBao
                {
                    NguoiDungId = customerId,
                    TieuDe = "Đã đăng bình luận thành công",
                    NoiDung = $"Bình luận của bạn trên sản phẩm giá trị đã được đăng thành công.",
                    DaDoc = rdThongBao.NextDouble() < 0.8,
                    NgayTao = binhLuan.NgayBinhLuan,
                    Loai = "PhanHoiBinhLuan",
                    LienKetId = binhLuan.Id
                });
            }

            _db.ThongBaos.AddRange(thongBaos);
            _db.SaveChanges();
        }

        private void SeedUserTreesAndJournals()
        {
            if (_db.CayCuaTois.Any())
            {
                return;
            }

            var users = _db.NguoiDungUngDungs.ToList();
            if (!users.Any()) return;

            var trees = new List<CayCuaToi>();
            var random = new Random();

            // Create trees for random users
            foreach (var user in users)
            {
                if (random.NextDouble() > 0.7) continue; // 30% users have trees

                int treeCount = random.Next(1, 4);
                for (int i = 0; i < treeCount; i++)
                {
                    trees.Add(new CayCuaToi
                    {
                        NguoiDungId = user.Id,
                        TenCay = $"Cây Bonsai {random.Next(100, 999)} của {user.HoTen}",
                        NgayMua = DateTime.UtcNow.AddDays(-random.Next(30, 365)),
                        TrangThai = random.NextDouble() > 0.2 ? "Khỏe mạnh" : "Cần chăm sóc",
                        GhiChu = "Cây đang phát triển tốt.",
                        HinhAnhDaiDien = RandomData_DB.Instance.RandomImageBonsai()
                    });
                }
            }

            _db.CayCuaTois.AddRange(trees);
            _db.SaveChanges();

            // Create journals for trees
            var journals = new List<NhatKyCay>();
            foreach (var tree in trees)
            {
                int journalCount = random.Next(2, 6);
                for (int j = 0; j < journalCount; j++)
                {
                    journals.Add(new NhatKyCay
                    {
                        CayCuaToiId = tree.Id,
                        NgayTao = DateTimeOffset.UtcNow.AddDays(-random.Next(1, 30) * j),
                        NoiDung = $"Nhật ký ngày {j + 1}: Cây đã ra thêm lá mới.",
                        HinhAnh = random.NextDouble() > 0.5 ? RandomData_DB.Instance.RandomImageBonsai() : null,
                        GiaiDoanPhatTrien = "Phát triển"
                    });
                }
            }

            _db.NhatKyCays.AddRange(journals);
            _db.SaveChanges();
        }

        private void SeedForumThreadsAndPosts()
        {
            if (_db.ChuDes.Any())
            {
                return;
            }

            var users = _db.NguoiDungUngDungs.ToList();
            var categories = _db.DanhMucDienDans.ToList();

            if (!users.Any() || !categories.Any()) return;

            var threads = new List<ChuDe>();
            var random = new Random();

            // Realistic Vietnamese Titles & Content
            var discussionTopics = new[]
            {
                new { Title = "Cách chăm sóc cây Sanh vào mùa đông?", Content = "<p>Chào mọi người, em mới chơi bonsai. Cho em hỏi cây Sanh vào mùa lạnh có cần chế độ chăm sóc đặc biệt nào không ạ? Em thấy lá nó rụng nhiều quá.</p>" },
                new { Title = "Khoe cây Mai chiếu thủy mới tậu", Content = "<p>Mới rước em này về hôm qua, giá cũng hơi chát nhưng ưng cái dáng. Các bác cho em xin ít gạch đá để hoàn thiện em nó nhé!</p><p><img src='https://images.unsplash.com/photo-1613143828771-08183182512a?auto=format&fit=crop&q=80&w=600' alt='Bonsai' /></p>" },
                new { Title = "Tư vấn kỹ thuật uốn cành", Content = "<p>Em đang tập uốn cành cho cây Tùng La Hán. Có bác nào có tài liệu hay video hướng dẫn chi tiết cho người mới bắt đầu không ạ? Em cảm ơn.</p>" },
                new { Title = "Phân bón nào tốt cho cây lá kim?", Content = "<p>Các bác thường dùng loại phân bón nào cho dòng cây lá kim vậy ạ? Em dùng NPK thường thấy không hiệu quả lắm.</p>" },
                new { Title = "Giao lưu anh em khu vực TP.HCM", Content = "<p>Cuối tuần này có anh em nào rảnh cafe giao lưu chia sẻ kinh nghiệm không ạ? Em ở khu vực Thủ Đức.</p>" },
                new { Title = "Cây bị vàng lá, cứu em với!", Content = "<p>Cây của em tự nhiên bị vàng lá hàng loạt, không biết có phải do tưới nước nhiều quá không. Bác nào bắt bệnh giúp em với :(.</p>" },
                new { Title = "Địa chỉ mua chậu bonsai đẹp giá rẻ", Content = "<p>Em đang cần tìm mua ít chậu gốm cho dàn cây mini. Bác nào biết chỗ bán uy tín ở Hà Nội chỉ em với ạ.</p>" }
            };

            var replyContents = new[]
            {
                "Cây đẹp quá bác ơi, chúc mừng bác!",
                "Theo kinh nghiệm của mình thì nên hạn chế tưới nước vào mùa đông nhé.",
                "Bác thử dùng phân hữu cơ xem, mình thấy tốt hơn NPK hóa học.",
                "Hóng cao nhân vào chỉ giáo, em cũng đang gặp vấn đề tương tự.",
                "Cuối tuần này em bận mất rồi, hẹn bác dịp khác nhé.",
                "Ra tiệm thuốc bảo vệ thực vật hỏi thuốc trị nấm xem sao bác.",
                "Chậu này mình hay mua ở chợ Bưởi, giá cũng phải chăng lắm."
            };

            // Create threads
            foreach (var topic in discussionTopics)
            {
                var user = users[random.Next(users.Count)];
                var category = categories[random.Next(categories.Count)]; // Assign distinct if valid, random for now

                // Override category for specific topics if needed, but random is fine for demo
                threads.Add(new ChuDe
                {
                    NguoiDungId = user.Id,
                    DanhMucDienDanId = category.Id,
                    TieuDe = topic.Title,
                    Slug = SD.GenerateSlug(topic.Title),
                    NoiDung = topic.Content,
                    LuotXem = random.Next(50, 1000),
                    NgayTao = DateTime.UtcNow.AddDays(-random.Next(1, 30)), // Fixed UtcNow
                    TrangThai = true
                });
            }

            _db.ChuDes.AddRange(threads);
            _db.SaveChanges();

            // Create posts for threads
            var posts = new List<BaiViet>();
            foreach (var thread in threads)
            {
                int postCount = random.Next(2, 6);
                for (int k = 0; k < postCount; k++)
                {
                    var commenter = users[random.Next(users.Count)];
                    posts.Add(new BaiViet
                    {
                        ChuDeId = thread.Id,
                        NguoiDungId = commenter.Id,
                        NoiDung = $"<p>{replyContents[random.Next(replyContents.Length)]}</p>",
                        NgayTao = thread.NgayTao.AddHours(random.Next(1, 48) * (k + 1)),
                        LaCauTraLoiDung = k == 0 && random.NextDouble() > 0.9 // Rare accepted answer
                    });
                }
            }

            _db.BaiViets.AddRange(posts);
            _db.SaveChanges();
        }

        public void SeedSampleData()
        {
            bool seedSampleData = _configuration.GetValue<bool>("SeedSampleData", true);

            if (!seedSampleData)
            {
                return;
            }

            try
            {
                SeedProductCategories();
                SeedProducts();
                SeedCombos();
                SeedComboDetails();
                SeedProductImages();
                SeedProductVideos();
                SeedForumCategories();

                SeedUsers();
                SeedOrders();
                SeedFavorites();
                SeedComments();
                SeedOrderDetails();

                SeedUserTreesAndJournals();
                SeedForumThreadsAndPosts();

                // Seed sample customer related data
                var customerUser = _db.NguoiDungUngDungs.FirstOrDefault(u => u.Email == "customer@dotnetmastery.com");
                if (customerUser != null)
                {
                    SeedSampleCustomerData(customerUser.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to seed sample data.");
                // Ensure app doesn't crash on seeding failure
            }
        }
    }
}
