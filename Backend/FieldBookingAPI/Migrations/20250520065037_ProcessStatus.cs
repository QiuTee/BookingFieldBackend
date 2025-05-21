using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class ProcessStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProcessStatus",
                table: "Bookings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessStatus",
                table: "Bookings");
        }
    }
}
