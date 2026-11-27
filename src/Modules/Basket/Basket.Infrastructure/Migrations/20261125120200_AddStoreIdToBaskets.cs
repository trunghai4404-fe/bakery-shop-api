using Microsoft.EntityFrameworkCore.Migrations;

namespace Basket.Infrastructure.Migrations;

public partial class AddStoreIdToBaskets : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "StoreId",
            table: "Baskets",
            type: "bigint",
            nullable: true
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "StoreId",
            table: "Baskets"
        );
    }
}
