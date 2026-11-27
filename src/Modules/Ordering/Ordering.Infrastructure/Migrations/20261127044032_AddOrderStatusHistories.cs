using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderStatusHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistories_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_OrderId",
                table: "OrderStatusHistories",
                column: "OrderId");

            migrationBuilder.Sql(
                """
                CREATE FUNCTION "RecordOrderStatusHistory"() RETURNS trigger
                LANGUAGE plpgsql AS $order_status_history$
                BEGIN
                    IF TG_OP = 'INSERT' THEN
                        INSERT INTO "OrderStatusHistories" ("OrderId", "Status", "CreatedTime")
                        VALUES (NEW."Id", NEW."Status", NEW."CreatedAt");
                    ELSIF OLD."Status" IS DISTINCT FROM NEW."Status" THEN
                        INSERT INTO "OrderStatusHistories" ("OrderId", "Status", "CreatedTime")
                        VALUES (NEW."Id", NEW."Status", clock_timestamp());
                    END IF;
                    RETURN NEW;
                END;
                $order_status_history$;
                """);

            // Install the trigger before backfilling so the migration's table lock prevents missed changes.
            migrationBuilder.Sql(
                """
                CREATE TRIGGER "TR_Orders_RecordStatusHistory"
                AFTER INSERT OR UPDATE OF "Status" ON "Orders"
                FOR EACH ROW EXECUTE FUNCTION "RecordOrderStatusHistory"();

                INSERT INTO "OrderStatusHistories" ("OrderId", "Status", "CreatedTime")
                SELECT "Id", 1, "CreatedAt" FROM "Orders";

                INSERT INTO "OrderStatusHistories" ("OrderId", "Status", "CreatedTime")
                SELECT "Id", 7, "CancelledAt" FROM "Orders" WHERE "CancelledAt" IS NOT NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER "TR_Orders_RecordStatusHistory" ON "Orders";
                DROP FUNCTION "RecordOrderStatusHistory"();
                """);

            migrationBuilder.DropTable(
                name: "OrderStatusHistories");
        }
    }
}
