using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace B3cBonsai.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<Microsoft.AspNetCore.Identity.IdentityUser>
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
        public DbSet<GioHang> GioHangs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
                {
                    property.SetValueConverter(
                        new ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        )
                    );
                }
            }

            base.OnModelCreating(modelBuilder);


            #region//Thay đổi cấu trúc tên bảng được tạo từ Identity !!!Chưa áp dụng!!!
            //aspNetUser table -> IdentityUsers
            /*
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<IdentityUser>(e => 
            {
                e.ToTable(name: "NguoiDung", schema: "identity"); // Đổi tên bảng thành "NguoiDung"
            });
            modelBuilder.Entity<IdentityRoleClaim<string>>(e =>
            {
                e.ToTable(name: "YeuCauVaiTro", schema: "identity"); // Đổi tên bảng thành "YeuCauVaiTro"
            });
            modelBuilder.Entity<IdentityRole>(e =>
            {
                e.ToTable(name: "VaiTro", schema: "identity"); // Đổi tên bảng thành "VaiTro"
            });
            modelBuilder.Entity<IdentityUserClaim<string>>(e =>
            {
                e.ToTable(name: "YeuCauDanhTinh", schema: "identity"); // Đổi tên bảng thành "YeuCauDanhTinh"
            });
            modelBuilder.Entity<IdentityUserLogin<string>>(e =>
            {
                e.ToTable(name: "DangNhap", schema: "identity"); // Đổi tên bảng thành "DangNhap"
            });
            modelBuilder.Entity<IdentityUserRole<string>>(e =>
            {
                e.ToTable(name: "VaiTroNguoiDung", schema: "identity"); // Đổi tên bảng thành "VaiTroNguoiDung"
            });
            modelBuilder.Entity<IdentityUserToken<string>>(e =>
            {
                e.ToTable(name: "TokenDangNhap", schema: "identity"); // Đổi tên bảng thành "TokenDangNhap"
            });*/
            #endregion
            /*
                        // Thiết lập quan hệ giữa NguoiDungUngDung với IdentityUser
                        modelBuilder.Entity<NguoiDungUngDung>()
                            .HasOne(n => n.NguoiDungUngDung);*/

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


        }
    }
}
