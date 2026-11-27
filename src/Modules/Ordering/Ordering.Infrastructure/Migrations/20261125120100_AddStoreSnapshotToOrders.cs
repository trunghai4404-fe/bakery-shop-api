using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Migrations;

[DbContext(typeof(OrderingDbContext))]
[Migration("20261125120100_AddStoreSnapshotToOrders")]
public partial class AddStoreSnapshotToOrders : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.AddColumn<string>(name: "StoreInfo", table: "Orders", type: "jsonb", nullable: true);

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropColumn(name: "StoreInfo", table: "Orders");
}
