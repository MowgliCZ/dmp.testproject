using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RssReaderApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLastUpdatedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Feeds",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Feeds");
        }
    }
}
