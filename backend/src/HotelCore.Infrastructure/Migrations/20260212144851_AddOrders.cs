using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "entity_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    property_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    old_value = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    new_value = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    changed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    change_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity_histories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    payment_type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    is_hidden = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_orders_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "my_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_my_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_my_images_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "order_clients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    guest_access_token = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_clients", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_clients_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_clients_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_entity_histories_changed_at",
                table: "entity_histories",
                column: "changed_at");

            migrationBuilder.CreateIndex(
                name: "ix_entity_histories_changed_by_user_id",
                table: "entity_histories",
                column: "changed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_EntityHistory_Entity_ChangedAt",
                table: "entity_histories",
                columns: new[] { "entity_type", "entity_id", "changed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_my_images_order_id_display_order_type",
                table: "my_images",
                columns: new[] { "order_id", "display_order", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_order_clients_email",
                table: "order_clients",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_order_clients_guest_access_token",
                table: "order_clients",
                column: "guest_access_token");

            migrationBuilder.CreateIndex(
                name: "ix_order_clients_order_id",
                table: "order_clients",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_clients_phone_number",
                table: "order_clients",
                column: "phone_number");

            migrationBuilder.CreateIndex(
                name: "ix_order_clients_user_id",
                table: "order_clients",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_created_by_user_id",
                table: "orders",
                column: "created_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entity_histories");

            migrationBuilder.DropTable(
                name: "my_images");

            migrationBuilder.DropTable(
                name: "order_clients");

            migrationBuilder.DropTable(
                name: "orders");
        }
    }
}
