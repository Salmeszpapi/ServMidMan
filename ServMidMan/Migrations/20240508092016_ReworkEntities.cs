using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServMidMan.Migrations
{
    /// <inheritdoc />
    public partial class ReworkEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Products",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Products");
        }
    }
}
