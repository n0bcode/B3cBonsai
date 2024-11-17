using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B3cBonsai.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addGioHangAndTrangThaiCombo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "ComboSanPhams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "GioHangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSanPham = table.Column<int>(type: "int", nullable: true),
                    MaCombo = table.Column<int>(type: "int", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    MaKhachHang = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoaiDoiTuong = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GioHangs");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "ComboSanPhams");
        }
    }
}
