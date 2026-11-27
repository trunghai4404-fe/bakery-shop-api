using Contents.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Contents.Infrastructure.Persistence.Migrations;

[DbContext(typeof(ContentsDbContext))]
[Migration("20261125120000_AddStores")]
public partial class AddStores : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Stores",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                ProvinceId = table.Column<int>(type: "integer", nullable: false),
                WardId = table.Column<int>(type: "integer", nullable: false),
                Latitude = table.Column<double>(type: "double precision", nullable: false),
                Longitude = table.Column<double>(type: "double precision", nullable: false),
                Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                OpeningHours = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                LastModifiedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Stores", x => x.Id));

        migrationBuilder.CreateIndex("IX_Stores_IsActive_SortOrder", "Stores", new[] { "IsActive", "SortOrder" });
        migrationBuilder.CreateIndex("IX_Stores_ProvinceId", "Stores", "ProvinceId");
        migrationBuilder.CreateIndex("IX_Stores_WardId", "Stores", "WardId");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Stores");
}
