using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MarkOrderHistoryCreationEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""LOCK TABLE "Orders" IN SHARE ROW EXCLUSIVE MODE;""");

            migrationBuilder.AddColumn<bool>(
                name: "IsOrderCreation",
                table: "OrderStatusHistories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "RecordOrderStatusHistory"() RETURNS trigger
                LANGUAGE plpgsql AS $order_status_history$
                BEGIN
                    IF TG_OP = 'INSERT' THEN
                        INSERT INTO "OrderStatusHistories" ("OrderId", "Status", "CreatedTime", "IsOrderCreation")
                        VALUES (NEW."Id", NEW."Status", NEW."CreatedAt", TRUE);
                    ELSIF OLD."Status" IS DISTINCT FROM NEW."Status" THEN
                        INSERT INTO "OrderStatusHistories" ("OrderId", "Status", "CreatedTime")
                        VALUES (NEW."Id", NEW."Status", clock_timestamp());
                    END IF;
                    RETURN NEW;
                END;
                $order_status_history$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                LOCK TABLE "Orders" IN SHARE ROW EXCLUSIVE MODE;

                CREATE OR REPLACE FUNCTION "RecordOrderStatusHistory"() RETURNS trigger
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

            migrationBuilder.DropColumn(
                name: "IsOrderCreation",
                table: "OrderStatusHistories");
        }
    }
}
