using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Services.Customers.Shared.Data.Migrations.Customer
{
    public partial class UpdateRestockSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "processed",
                schema: "customer",
                table: "restock_subscriptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "processed_time",
                schema: "customer",
                table: "restock_subscriptions",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "processed",
                schema: "customer",
                table: "restock_subscriptions");

            migrationBuilder.DropColumn(
                name: "processed_time",
                schema: "customer",
                table: "restock_subscriptions");
        }
    }
}
