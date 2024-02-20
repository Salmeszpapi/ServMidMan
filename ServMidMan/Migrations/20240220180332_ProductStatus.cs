using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServMidMan.Migrations
{
    /// <inheritdoc />
    public partial class ProductStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "productStatus",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "productStatus",
                table: "Services");
        }
    }
}
