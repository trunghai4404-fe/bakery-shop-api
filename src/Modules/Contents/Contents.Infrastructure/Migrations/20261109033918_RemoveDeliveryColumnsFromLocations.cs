using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeliveryColumnsFromLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wards_IsDeliverable",
                table: "Wards");

            migrationBuilder.DropIndex(
                name: "IX_Districts_IsDeliverable",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "IsDeliverable",
                table: "Wards");

            migrationBuilder.DropColumn(
                name: "IsDeliverable",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "ShippingFee",
                table: "Districts");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_SortOrder",
                table: "Wards",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_SortOrder",
                table: "Districts",
                column: "SortOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wards_SortOrder",
                table: "Wards");

            migrationBuilder.DropIndex(
                name: "IX_Districts_SortOrder",
                table: "Districts");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeliverable",
                table: "Wards",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeliverable",
                table: "Districts",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingFee",
                table: "Districts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Wards_IsDeliverable",
                table: "Wards",
                column: "IsDeliverable");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_IsDeliverable",
                table: "Districts",
                column: "IsDeliverable");
        }
    }
}
