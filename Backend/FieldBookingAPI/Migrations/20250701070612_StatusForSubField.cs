﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class StatusForSubField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SubFields",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "SubFields");
        }
    }
}
