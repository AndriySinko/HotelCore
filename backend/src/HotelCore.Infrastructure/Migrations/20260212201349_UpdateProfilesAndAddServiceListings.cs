using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfilesAndAddServiceListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_requests_users_client_id",
                table: "work_requests");

            migrationBuilder.RenameColumn(
                name: "client_id",
                table: "work_requests",
                newName: "seeker_profile_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_requests_client_id",
                table: "work_requests",
                newName: "ix_work_requests_seeker_profile_id");

            migrationBuilder.CreateTable(
                name: "seeker_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bio = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    reviews_count = table.Column<int>(type: "integer", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    default_location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seeker_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_seeker_profiles_locations_default_location_id",
                        column: x => x.default_location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_seeker_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worker_service_listings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    worker_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    starting_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tags = table.Column<string[]>(type: "text[]", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_worker_service_listings", x => x.id);
                    table.ForeignKey(
                        name: "fk_worker_service_listings_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_worker_service_listings_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_worker_service_listings_worker_profiles_worker_profile_id",
                        column: x => x.worker_profile_id,
                        principalTable: "worker_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_seeker_profiles_default_location_id",
                table: "seeker_profiles",
                column: "default_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_seeker_profiles_user_id",
                table: "seeker_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_worker_service_listings_category_id",
                table: "worker_service_listings",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_worker_service_listings_location_id",
                table: "worker_service_listings",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_worker_service_listings_worker_profile_id",
                table: "worker_service_listings",
                column: "worker_profile_id");

            migrationBuilder.AddForeignKey(
                name: "fk_work_requests_seeker_profiles_seeker_profile_id",
                table: "work_requests",
                column: "seeker_profile_id",
                principalTable: "seeker_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_requests_seeker_profiles_seeker_profile_id",
                table: "work_requests");

            migrationBuilder.DropTable(
                name: "seeker_profiles");

            migrationBuilder.DropTable(
                name: "worker_service_listings");

            migrationBuilder.RenameColumn(
                name: "seeker_profile_id",
                table: "work_requests",
                newName: "client_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_requests_seeker_profile_id",
                table: "work_requests",
                newName: "ix_work_requests_client_id");

            migrationBuilder.AddForeignKey(
                name: "fk_work_requests_users_client_id",
                table: "work_requests",
                column: "client_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
