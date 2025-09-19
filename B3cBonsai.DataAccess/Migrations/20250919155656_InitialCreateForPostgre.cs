using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace B3cBonsai.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateForPostgre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    HoTen = table.Column<string>(type: "character varying(54)", maxLength: 54, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SoDienThoai = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    GioiTinh = table.Column<bool>(type: "boolean", nullable: true),
                    CCCD = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    DiaChi = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    LinkAnh = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MoTa = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComboSanPhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenCombo = table.Column<string>(type: "character varying(54)", maxLength: 54, nullable: false),
                    MoTa = table.Column<string>(type: "text", nullable: false),
                    LinkAnh = table.Column<string>(type: "text", nullable: true),
                    GiamGia = table.Column<int>(type: "integer", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    TongGia = table.Column<int>(type: "integer", nullable: false),
                    TrangThai = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboSanPhams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucSanPhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenDanhMuc = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucSanPhams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonHangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NguoiDungId = table.Column<string>(type: "text", nullable: false),
                    NhanVienId = table.Column<string>(type: "text", nullable: true),
                    NgayDatHang = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayNhanHang = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TongTienDonHang = table.Column<double>(type: "double precision", nullable: false),
                    TrangThaiDonHang = table.Column<string>(type: "text", nullable: true),
                    TrangThaiThanhToan = table.Column<string>(type: "text", nullable: true),
                    SoTheoDoi = table.Column<string>(type: "text", nullable: true),
                    NhaVanChuyen = table.Column<string>(type: "text", nullable: true),
                    NgayThanhToan = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NgayHetHanThanhToan = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaPhienThanhToan = table.Column<string>(type: "text", nullable: true),
                    MaYeuCauThanhToan = table.Column<string>(type: "text", nullable: true),
                    SoDienThoai = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    Duong = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ThanhPho = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tinh = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MaBuuDien = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TenNguoiNhan = table.Column<string>(type: "character varying(54)", maxLength: 54, nullable: false),
                    LyDoHuyDonHang = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonHangs_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_AspNetUsers_NhanVienId",
                        column: x => x.NhanVienId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SanPhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenSanPham = table.Column<string>(type: "character varying(54)", maxLength: 54, nullable: false),
                    DanhMucId = table.Column<int>(type: "integer", nullable: false),
                    MoTa = table.Column<string>(type: "text", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    Gia = table.Column<int>(type: "integer", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgaySuaDoi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrangThai = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SanPhams_DanhMucSanPhams_DanhMucId",
                        column: x => x.DanhMucId,
                        principalTable: "DanhMucSanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BinhLuans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NoiDungBinhLuan = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    NguoiDungId = table.Column<string>(type: "text", nullable: false),
                    SanPhamId = table.Column<int>(type: "integer", nullable: false),
                    NgayBinhLuan = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TinhTrang = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinhLuans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BinhLuans_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BinhLuans_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietCombos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComboId = table.Column<int>(type: "integer", nullable: false),
                    SanPhamId = table.Column<int>(type: "integer", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietCombos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietCombos_ComboSanPhams_ComboId",
                        column: x => x.ComboId,
                        principalTable: "ComboSanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietCombos_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DonHangId = table.Column<int>(type: "integer", nullable: false),
                    SanPhamId = table.Column<int>(type: "integer", nullable: true),
                    ComboId = table.Column<int>(type: "integer", nullable: true),
                    LoaiDoiTuong = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    Gia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHangs_ComboSanPhams_ComboId",
                        column: x => x.ComboId,
                        principalTable: "ComboSanPhams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                        column: x => x.DonHangId,
                        principalTable: "DonHangs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHangs_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GioHangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaSanPham = table.Column<int>(type: "integer", nullable: true),
                    MaCombo = table.Column<int>(type: "integer", nullable: true),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    MaKhachHang = table.Column<string>(type: "text", nullable: false),
                    LoaiDoiTuong = table.Column<string>(type: "text", nullable: false),
                    LinkAnh = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GioHangs_AspNetUsers_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GioHangs_ComboSanPhams_MaCombo",
                        column: x => x.MaCombo,
                        principalTable: "ComboSanPhams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GioHangs_SanPhams_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "SanPhams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HinhAnhSanPhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LinkAnh = table.Column<string>(type: "text", nullable: false),
                    SanPhamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhSanPhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HinhAnhSanPhams_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoSanPhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenVideo = table.Column<string>(type: "character varying(54)", maxLength: 54, nullable: false),
                    LinkVideo = table.Column<string>(type: "text", nullable: false),
                    SanPhamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoSanPhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoSanPhams_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhSachYeuThichs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SanPhamId = table.Column<int>(type: "integer", nullable: true),
                    ComboId = table.Column<int>(type: "integer", nullable: true),
                    BinhLuanId = table.Column<int>(type: "integer", nullable: true),
                    NguoiDungId = table.Column<string>(type: "text", nullable: false),
                    LoaiDoiTuong = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NgayYeuThich = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhSachYeuThichs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhSachYeuThichs_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhSachYeuThichs_BinhLuans_BinhLuanId",
                        column: x => x.BinhLuanId,
                        principalTable: "BinhLuans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DanhSachYeuThichs_ComboSanPhams_ComboId",
                        column: x => x.ComboId,
                        principalTable: "ComboSanPhams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DanhSachYeuThichs_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuans_NguoiDungId",
                table: "BinhLuans",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuans_SanPhamId",
                table: "BinhLuans",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietCombos_ComboId",
                table: "ChiTietCombos",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietCombos_SanPhamId",
                table: "ChiTietCombos",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_ComboId",
                table: "ChiTietDonHangs",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_DonHangId",
                table: "ChiTietDonHangs",
                column: "DonHangId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_SanPhamId",
                table: "ChiTietDonHangs",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhSachYeuThichs_BinhLuanId",
                table: "DanhSachYeuThichs",
                column: "BinhLuanId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhSachYeuThichs_ComboId",
                table: "DanhSachYeuThichs",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhSachYeuThichs_NguoiDungId",
                table: "DanhSachYeuThichs",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhSachYeuThichs_SanPhamId",
                table: "DanhSachYeuThichs",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_NguoiDungId",
                table: "DonHangs",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_NhanVienId",
                table: "DonHangs",
                column: "NhanVienId");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_MaCombo",
                table: "GioHangs",
                column: "MaCombo");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_MaKhachHang",
                table: "GioHangs",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_MaSanPham",
                table: "GioHangs",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhSanPhams_SanPhamId",
                table: "HinhAnhSanPhams",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_DanhMucId",
                table: "SanPhams",
                column: "DanhMucId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoSanPhams_SanPhamId",
                table: "VideoSanPhams",
                column: "SanPhamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChiTietCombos");

            migrationBuilder.DropTable(
                name: "ChiTietDonHangs");

            migrationBuilder.DropTable(
                name: "DanhSachYeuThichs");

            migrationBuilder.DropTable(
                name: "GioHangs");

            migrationBuilder.DropTable(
                name: "HinhAnhSanPhams");

            migrationBuilder.DropTable(
                name: "VideoSanPhams");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DonHangs");

            migrationBuilder.DropTable(
                name: "BinhLuans");

            migrationBuilder.DropTable(
                name: "ComboSanPhams");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SanPhams");

            migrationBuilder.DropTable(
                name: "DanhMucSanPhams");
        }
    }
}
