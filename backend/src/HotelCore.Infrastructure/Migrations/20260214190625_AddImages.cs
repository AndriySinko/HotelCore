using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_my_images_orders_order_id",
                table: "my_images");

            migrationBuilder.DropIndex(
                name: "ix_my_images_order_id_display_order_type",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "caption",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "order_id",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "display_order",
                table: "my_images",
                newName: "width");

            migrationBuilder.AddColumn<double>(
                name: "aspect_ratio",
                table: "my_images",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "height",
                table: "my_images",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "image_group_id",
                table: "my_images",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "size_bytes",
                table: "my_images",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "storage_key",
                table: "my_images",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "avatar_id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "my_image_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    position = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_my_image_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_my_image_groups_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_my_image_groups_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_my_images_image_group_id_type",
                table: "my_images",
                columns: new[] { "image_group_id", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_avatar_id",
                table: "AspNetUsers",
                column: "avatar_id");

            migrationBuilder.CreateIndex(
                name: "ix_my_image_groups_order_id_position",
                table: "my_image_groups",
                columns: new[] { "order_id", "position" });

            migrationBuilder.CreateIndex(
                name: "ix_my_image_groups_user_id",
                table: "my_image_groups",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_my_image_groups_avatar_id",
                table: "AspNetUsers",
                column: "avatar_id",
                principalTable: "my_image_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_my_images_my_image_groups_image_group_id",
                table: "my_images",
                column: "image_group_id",
                principalTable: "my_image_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_my_image_groups_avatar_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "fk_my_images_my_image_groups_image_group_id",
                table: "my_images");

            migrationBuilder.DropTable(
                name: "my_image_groups");

            migrationBuilder.DropIndex(
                name: "ix_my_images_image_group_id_type",
                table: "my_images");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_avatar_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "aspect_ratio",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "height",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "image_group_id",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "size_bytes",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "storage_key",
                table: "my_images");

            migrationBuilder.DropColumn(
                name: "avatar_id",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "width",
                table: "my_images",
                newName: "display_order");

            migrationBuilder.AddColumn<string>(
                name: "caption",
                table: "my_images",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "my_images",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "my_images",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "my_images",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "order_id",
                table: "my_images",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "my_images",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                table: "AspNetUsers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_my_images_order_id_display_order_type",
                table: "my_images",
                columns: new[] { "order_id", "display_order", "type" });

            migrationBuilder.AddForeignKey(
                name: "fk_my_images_orders_order_id",
                table: "my_images",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id");
        }
    }
}
