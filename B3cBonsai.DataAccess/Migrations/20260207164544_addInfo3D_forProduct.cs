using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B3cBonsai.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addInfo3D_forProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsARReady",
                table: "SanPhams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Model3DMetadata",
                table: "SanPhams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model3DPath",
                table: "SanPhams",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsARReady",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "Model3DMetadata",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "Model3DPath",
                table: "SanPhams");
        }
    }
}
