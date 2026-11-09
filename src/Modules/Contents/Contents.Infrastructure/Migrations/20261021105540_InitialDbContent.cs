using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDbContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Banners",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Banners_SortOrder",
                table: "Banners",
                column: "SortOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Banners_SortOrder",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Banners");
        }
    }
}
