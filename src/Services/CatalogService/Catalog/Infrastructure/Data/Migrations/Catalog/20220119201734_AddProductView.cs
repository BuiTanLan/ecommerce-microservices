using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Catalog.Infrastructure.Data.Migrations.Catalog
{
    public partial class AddProductView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product_views",
                schema: "catalog",
                columns: table => new
                {
                    product_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_name = table.Column<string>(type: "text", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    category_name = table.Column<string>(type: "text", nullable: true),
                    supplier_id = table.Column<long>(type: "bigint", nullable: true),
                    supplier_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_views", x => x.product_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_views_product_id",
                schema: "catalog",
                table: "product_views",
                column: "product_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_views",
                schema: "catalog");
        }
    }
}
