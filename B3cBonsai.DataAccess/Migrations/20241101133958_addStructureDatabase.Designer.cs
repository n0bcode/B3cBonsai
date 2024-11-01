﻿// <auto-generated />
using System;
using B3cBonsai.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace B3cBonsai.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241101133958_addStructureDatabase")]
    partial class addStructureDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("B3cBonsai.Models.BinhLuan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("NguoiDungId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("NoiDungBinhLuan")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("SanPhamId")
                        .HasColumnType("int");

                    b.Property<bool>("TinhTrang")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("NguoiDungId");

                    b.HasIndex("SanPhamId");

                    b.ToTable("BinhLuans");
                });

            modelBuilder.Entity("B3cBonsai.Models.ChiTietCombo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ComboId")
                        .HasColumnType("int");

                    b.Property<int>("SanPhamId")
                        .HasColumnType("int");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ComboId");

                    b.HasIndex("SanPhamId");

                    b.ToTable("ChiTietCombos");
                });

            modelBuilder.Entity("B3cBonsai.Models.ChiTietDonHang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ComboId")
                        .HasColumnType("int");

                    b.Property<int>("DonHangId")
                        .HasColumnType("int");

                    b.Property<int>("Gia")
                        .HasColumnType("int");

                    b.Property<string>("LoaiSanPham")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int?>("SanPhamId")
                        .HasColumnType("int");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ComboId");

                    b.HasIndex("DonHangId");

                    b.HasIndex("SanPhamId");

                    b.ToTable("ChiTietDonHangs");
                });

            modelBuilder.Entity("B3cBonsai.Models.ComboSanPham", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("GiamGia")
                        .HasColumnType("int");

                    b.Property<string>("MoTa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenCombo")
                        .IsRequired()
                        .HasMaxLength(54)
                        .HasColumnType("nvarchar(54)");

                    b.Property<int>("TongGia")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ComboSanPhams");
                });

            modelBuilder.Entity("B3cBonsai.Models.DanhMucSanPham", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("TenDanhMuc")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("DanhMucSanPhams");
                });

            modelBuilder.Entity("B3cBonsai.Models.DanhSachYeuThich", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BinhLuanId")
                        .HasColumnType("int");

                    b.Property<string>("LoaiDoiTuong")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("NguoiDungId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("SanPhamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BinhLuanId");

                    b.HasIndex("NguoiDungId");

                    b.HasIndex("SanPhamId");

                    b.ToTable("DanhSachYeuThichs");
                });

            modelBuilder.Entity("B3cBonsai.Models.DonHang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DiaChi")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("LyDoHuy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("NgayDatHang")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayNhanHang")
                        .HasColumnType("datetime2");

                    b.Property<string>("NguoiDungId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("NhanVienId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PhuongThucThanhToan")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("SoDienThoai")
                        .IsRequired()
                        .HasMaxLength(18)
                        .HasColumnType("nvarchar(18)");

                    b.Property<string>("TenNguoiNhan")
                        .IsRequired()
                        .HasMaxLength(54)
                        .HasColumnType("nvarchar(54)");

                    b.Property<string>("TrangThai")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.HasIndex("NguoiDungId");

                    b.HasIndex("NhanVienId");

                    b.ToTable("DonHangs");
                });

            modelBuilder.Entity("B3cBonsai.Models.HinhAnhSanPham", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("LinkAnh")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SanPhamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SanPhamId");

                    b.ToTable("HinhAnhSanPhams");
                });

            modelBuilder.Entity("B3cBonsai.Models.NguoiDungUngDung", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CCCD")
                        .HasMaxLength(18)
                        .HasColumnType("nvarchar(18)");

                    b.Property<string>("DiaChi")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(54)
                        .HasColumnType("nvarchar(54)");

                    b.Property<string>("HoTen")
                        .IsRequired()
                        .HasMaxLength(54)
                        .HasColumnType("nvarchar(54)");

                    b.Property<string>("LinkAnh")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("MatKhau")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("NgaySinh")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("SoDienThoai")
                        .IsRequired()
                        .HasMaxLength(18)
                        .HasColumnType("nvarchar(18)");

                    b.Property<string>("TenDangNhap")
                        .IsRequired()
                        .HasMaxLength(18)
                        .HasColumnType("nvarchar(18)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.Property<string>("VaiTro")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<bool>("XacThucEmail")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("NguoiDungUngDungs");
                });

            modelBuilder.Entity("B3cBonsai.Models.SanPham", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DanhMucId")
                        .HasColumnType("int");

                    b.Property<int>("Gia")
                        .HasColumnType("int");

                    b.Property<string>("MoTa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("NgaySuaDoi")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.Property<string>("TenSanPham")
                        .IsRequired()
                        .HasMaxLength(54)
                        .HasColumnType("nvarchar(54)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("DanhMucId");

                    b.ToTable("SanPhams");
                });

            modelBuilder.Entity("B3cBonsai.Models.VideoSanPham", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("LinkVideo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SanPhamId")
                        .HasColumnType("int");

                    b.Property<string>("TenVideo")
                        .IsRequired()
                        .HasMaxLength(54)
                        .HasColumnType("nvarchar(54)");

                    b.HasKey("Id");

                    b.HasIndex("SanPhamId");

                    b.ToTable("VideoSanPhams");
                });

            modelBuilder.Entity("B3cBonsai.Models.BinhLuan", b =>
                {
                    b.HasOne("B3cBonsai.Models.NguoiDungUngDung", "NguoiDungUngDung")
                        .WithMany("BinhLuans")
                        .HasForeignKey("NguoiDungId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("B3cBonsai.Models.SanPham", "SanPham")
                        .WithMany("BinhLuans")
                        .HasForeignKey("SanPhamId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("NguoiDungUngDung");

                    b.Navigation("SanPham");
                });

            modelBuilder.Entity("B3cBonsai.Models.ChiTietCombo", b =>
                {
                    b.HasOne("B3cBonsai.Models.ComboSanPham", "Combo")
                        .WithMany("ChiTietCombos")
                        .HasForeignKey("ComboId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("B3cBonsai.Models.SanPham", "SanPham")
                        .WithMany("ChiTietCombos")
                        .HasForeignKey("SanPhamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Combo");

                    b.Navigation("SanPham");
                });

            modelBuilder.Entity("B3cBonsai.Models.ChiTietDonHang", b =>
                {
                    b.HasOne("B3cBonsai.Models.ComboSanPham", "Combo")
                        .WithMany()
                        .HasForeignKey("ComboId");

                    b.HasOne("B3cBonsai.Models.DonHang", "DonHang")
                        .WithMany("ChiTietDonHangs")
                        .HasForeignKey("DonHangId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("B3cBonsai.Models.SanPham", "SanPham")
                        .WithMany("ChiTietDonHangs")
                        .HasForeignKey("SanPhamId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Combo");

                    b.Navigation("DonHang");

                    b.Navigation("SanPham");
                });

            modelBuilder.Entity("B3cBonsai.Models.DanhSachYeuThich", b =>
                {
                    b.HasOne("B3cBonsai.Models.BinhLuan", "BinhLuan")
                        .WithMany()
                        .HasForeignKey("BinhLuanId");

                    b.HasOne("B3cBonsai.Models.NguoiDungUngDung", "NguoiDungUngDung")
                        .WithMany("DanhSachYeuThichs")
                        .HasForeignKey("NguoiDungId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("B3cBonsai.Models.SanPham", "SanPham")
                        .WithMany("DanhSachYeuThichs")
                        .HasForeignKey("SanPhamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("BinhLuan");

                    b.Navigation("NguoiDungUngDung");

                    b.Navigation("SanPham");
                });

            modelBuilder.Entity("B3cBonsai.Models.DonHang", b =>
                {
                    b.HasOne("B3cBonsai.Models.NguoiDungUngDung", "NguoiDungUngDung")
                        .WithMany("DonHangs")
                        .HasForeignKey("NguoiDungId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("B3cBonsai.Models.NguoiDungUngDung", "NhanVien")
                        .WithMany()
                        .HasForeignKey("NhanVienId");

                    b.Navigation("NguoiDungUngDung");

                    b.Navigation("NhanVien");
                });

            modelBuilder.Entity("B3cBonsai.Models.HinhAnhSanPham", b =>
                {
                    b.HasOne("B3cBonsai.Models.SanPham", "SanPham")
                        .WithMany("HinhAnhs")
                        .HasForeignKey("SanPhamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SanPham");
                });

            modelBuilder.Entity("B3cBonsai.Models.SanPham", b =>
                {
                    b.HasOne("B3cBonsai.Models.DanhMucSanPham", "DanhMuc")
                        .WithMany("SanPhams")
                        .HasForeignKey("DanhMucId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DanhMuc");
                });

            modelBuilder.Entity("B3cBonsai.Models.VideoSanPham", b =>
                {
                    b.HasOne("B3cBonsai.Models.SanPham", "SanPham")
                        .WithMany("Videos")
                        .HasForeignKey("SanPhamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SanPham");
                });

            modelBuilder.Entity("B3cBonsai.Models.ComboSanPham", b =>
                {
                    b.Navigation("ChiTietCombos");
                });

            modelBuilder.Entity("B3cBonsai.Models.DanhMucSanPham", b =>
                {
                    b.Navigation("SanPhams");
                });

            modelBuilder.Entity("B3cBonsai.Models.DonHang", b =>
                {
                    b.Navigation("ChiTietDonHangs");
                });

            modelBuilder.Entity("B3cBonsai.Models.NguoiDungUngDung", b =>
                {
                    b.Navigation("BinhLuans");

                    b.Navigation("DanhSachYeuThichs");

                    b.Navigation("DonHangs");
                });

            modelBuilder.Entity("B3cBonsai.Models.SanPham", b =>
                {
                    b.Navigation("BinhLuans");

                    b.Navigation("ChiTietCombos");

                    b.Navigation("ChiTietDonHangs");

                    b.Navigation("DanhSachYeuThichs");

                    b.Navigation("HinhAnhs");

                    b.Navigation("Videos");
                });
#pragma warning restore 612, 618
        }
    }
}
