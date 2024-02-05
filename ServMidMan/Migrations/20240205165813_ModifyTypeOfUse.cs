using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServMidMan.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTypeOfUse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "TestImage",
                table: "Images");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Images",
                type: "longblob",
                nullable: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "TestImage",
                table: "Images",
                type: "longblob",
                nullable: false);
        }
    }
}
