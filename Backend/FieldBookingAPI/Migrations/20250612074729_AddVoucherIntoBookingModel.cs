using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherIntoBookingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountAmount",
                table: "Bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherCode",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoucherId",
                table: "Bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VoucherId",
                table: "Bookings",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Vouchers_VoucherId",
                table: "Bookings",
                column: "VoucherId",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Vouchers_VoucherId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_VoucherId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "VoucherCode",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "VoucherId",
                table: "Bookings");
        }
    }
}
