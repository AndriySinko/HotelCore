using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "parent_order_id",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "ix_orders_parent_order_id",
                table: "orders",
                column: "parent_order_id");

            migrationBuilder.AddForeignKey(
                name: "fk_orders_orders_parent_order_id",
                table: "orders",
                column: "parent_order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_orders_parent_order_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_parent_order_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "parent_order_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "type",
                table: "orders");
        }
    }
}
