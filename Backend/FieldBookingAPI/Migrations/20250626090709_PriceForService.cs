﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class PriceForService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "FieldServices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "FieldServices");
        }
    }
}
