using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B3cBonsai.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TenMigrationMoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "VideoSanPhams",
                newName: "VideoSanPhams",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SanPhams",
                newName: "SanPhams",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "NguoiDungUngDungs",
                newName: "NguoiDungUngDungs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "HinhAnhSanPhams",
                newName: "HinhAnhSanPhams",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "DonHangs",
                newName: "DonHangs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "DanhSachYeuThichs",
                newName: "DanhSachYeuThichs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "DanhMucSanPhams",
                newName: "DanhMucSanPhams",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ComboSanPhams",
                newName: "ComboSanPhams",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ChiTietDonHangs",
                newName: "ChiTietDonHangs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ChiTietCombos",
                newName: "ChiTietCombos",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "BinhLuans",
                newName: "BinhLuans",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "dbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "VideoSanPhams",
                schema: "dbo",
                newName: "VideoSanPhams");

            migrationBuilder.RenameTable(
                name: "SanPhams",
                schema: "dbo",
                newName: "SanPhams");

            migrationBuilder.RenameTable(
                name: "NguoiDungUngDungs",
                schema: "dbo",
                newName: "NguoiDungUngDungs");

            migrationBuilder.RenameTable(
                name: "HinhAnhSanPhams",
                schema: "dbo",
                newName: "HinhAnhSanPhams");

            migrationBuilder.RenameTable(
                name: "DonHangs",
                schema: "dbo",
                newName: "DonHangs");

            migrationBuilder.RenameTable(
                name: "DanhSachYeuThichs",
                schema: "dbo",
                newName: "DanhSachYeuThichs");

            migrationBuilder.RenameTable(
                name: "DanhMucSanPhams",
                schema: "dbo",
                newName: "DanhMucSanPhams");

            migrationBuilder.RenameTable(
                name: "ComboSanPhams",
                schema: "dbo",
                newName: "ComboSanPhams");

            migrationBuilder.RenameTable(
                name: "ChiTietDonHangs",
                schema: "dbo",
                newName: "ChiTietDonHangs");

            migrationBuilder.RenameTable(
                name: "ChiTietCombos",
                schema: "dbo",
                newName: "ChiTietCombos");

            migrationBuilder.RenameTable(
                name: "BinhLuans",
                schema: "dbo",
                newName: "BinhLuans");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "dbo",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "dbo",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "dbo",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "dbo",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "dbo",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "dbo",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "dbo",
                newName: "AspNetRoleClaims");
        }
    }
}
