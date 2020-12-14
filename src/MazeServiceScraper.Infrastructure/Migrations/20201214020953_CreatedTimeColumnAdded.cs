using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MazeServiceScraper.Infrastructure.Migrations
{
    public partial class CreatedTimeColumnAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CastId",
                table: "Casts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Casts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CastId",
                table: "Casts");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Casts");
        }
    }
}
