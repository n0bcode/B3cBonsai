using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace B3cBonsai.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddJournalAndForum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CayCuaTois",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NguoiDungId = table.Column<string>(type: "text", nullable: false),
                    SanPhamId = table.Column<int>(type: "integer", nullable: true),
                    TenCay = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NgayMua = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TrangThai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    HinhAnhDaiDien = table.Column<string>(type: "text", nullable: true),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CayCuaTois", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CayCuaTois_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CayCuaTois_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucDienDans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenDanhMuc = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucDienDans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhatKyCays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CayCuaToiId = table.Column<int>(type: "integer", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    HinhAnh = table.Column<string>(type: "text", nullable: true),
                    GiaiDoanPhatTrien = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyCays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhatKyCays_CayCuaTois_CayCuaToiId",
                        column: x => x.CayCuaToiId,
                        principalTable: "CayCuaTois",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChuDes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NguoiDungId = table.Column<string>(type: "text", nullable: false),
                    DanhMucDienDanId = table.Column<int>(type: "integer", nullable: false),
                    TieuDe = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    LuotXem = table.Column<int>(type: "integer", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Slug = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TrangThai = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuDes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChuDes_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChuDes_DanhMucDienDans_DanhMucDienDanId",
                        column: x => x.DanhMucDienDanId,
                        principalTable: "DanhMucDienDans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaiViets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChuDeId = table.Column<int>(type: "integer", nullable: false),
                    NguoiDungId = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    LaCauTraLoiDung = table.Column<bool>(type: "boolean", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiViets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaiViets_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BaiViets_ChuDes_ChuDeId",
                        column: x => x.ChuDeId,
                        principalTable: "ChuDes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaiViets_ChuDeId",
                table: "BaiViets",
                column: "ChuDeId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiViets_NguoiDungId",
                table: "BaiViets",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_CayCuaTois_NguoiDungId",
                table: "CayCuaTois",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_CayCuaTois_SanPhamId",
                table: "CayCuaTois",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_ChuDes_DanhMucDienDanId",
                table: "ChuDes",
                column: "DanhMucDienDanId");

            migrationBuilder.CreateIndex(
                name: "IX_ChuDes_NguoiDungId",
                table: "ChuDes",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKyCays_CayCuaToiId",
                table: "NhatKyCays",
                column: "CayCuaToiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaiViets");

            migrationBuilder.DropTable(
                name: "NhatKyCays");

            migrationBuilder.DropTable(
                name: "ChuDes");

            migrationBuilder.DropTable(
                name: "CayCuaTois");

            migrationBuilder.DropTable(
                name: "DanhMucDienDans");
        }
    }
}
