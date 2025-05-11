using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByAdminToField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByAdminId",
                table: "Fields",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fields_CreatedByAdminId",
                table: "Fields",
                column: "CreatedByAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Users_CreatedByAdminId",
                table: "Fields",
                column: "CreatedByAdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Users_CreatedByAdminId",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Fields_CreatedByAdminId",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "CreatedByAdminId",
                table: "Fields");
        }
    }
}
