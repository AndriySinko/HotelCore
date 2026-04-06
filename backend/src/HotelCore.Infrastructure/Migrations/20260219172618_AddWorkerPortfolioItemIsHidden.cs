using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerPortfolioItemIsHidden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_hidden",
                table: "worker_portfolio_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_hidden",
                table: "worker_portfolio_items");
        }
    }
}
