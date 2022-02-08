using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Services.Customers.Shared.Data.Migrations.Customer
{
    public partial class UpdateRestockSubscriptions4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "customer",
                table: "restock_subscriptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "customer",
                table: "restock_subscriptions");
        }
    }
}
