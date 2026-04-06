using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerPortfolioItemImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "worker_portfolio_item_id",
                table: "my_image_groups",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_my_image_groups_worker_portfolio_item_id_position",
                table: "my_image_groups",
                columns: new[] { "worker_portfolio_item_id", "position" });

            migrationBuilder.AddForeignKey(
                name: "fk_my_image_groups_worker_portfolio_items_worker_portfolio_ite",
                table: "my_image_groups",
                column: "worker_portfolio_item_id",
                principalTable: "worker_portfolio_items",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_my_image_groups_worker_portfolio_items_worker_portfolio_ite",
                table: "my_image_groups");

            migrationBuilder.DropIndex(
                name: "ix_my_image_groups_worker_portfolio_item_id_position",
                table: "my_image_groups");

            migrationBuilder.DropColumn(
                name: "worker_portfolio_item_id",
                table: "my_image_groups");
        }
    }
}
